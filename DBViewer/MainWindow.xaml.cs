using LevelDB;
using Microsoft.WindowsAPICodePack.Dialogs;
using ModernWpf.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DBViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<KeyItem> keysDataSource { get; set; }

        private readonly OpenFileDialog dialog = new OpenFileDialog();

        private KeyItem overworldGroup;
        private KeyItem netherGroup;
        private KeyItem endGroup;
        private KeyItem generalKeys;

        private Dictionary<ChunkPos, KeyItem> chunks = new Dictionary<ChunkPos, KeyItem>();

        private DB database;
        private byte[] currentKey;
        private byte[] currentValue;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            keysDataSource = new ObservableCollection<KeyItem>();

        }

        private void OnDatabaseOpen()
        {
            keysDataSource.Add(overworldGroup = new KeyItem { Name = "Chunks (Overworld)" });
            keysDataSource.Add(netherGroup = new KeyItem { Name = "Chunks (Nether)" });
            keysDataSource.Add(endGroup = new KeyItem { Name = "Chunks (End)" });
            keysDataSource.Add(generalKeys = new KeyItem { Name = "General Keys", IsExpanded = true });
            database = new DB(dialog.Options, dialog.FileName);
            var iterator = database.GetEnumerator();
            while (iterator.MoveNext())
            {
                var pair = iterator.Current;
                tryParseKeyAndAdd(pair.Key);
            }
            openFileMenu.Header = "Close";
        }

        private void OnDatabaseClose()
        {
            keysDataSource.Clear();
            chunks.Clear();
            database.Dispose();
            database = null;
            currentKey = null;
            currentValue = null;
            UpdateTextBox();
            openFileMenu.Header = "Open";
        }

        private async void OnMenuOpenClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("OnMenuOpenClick");
            if (database == null)
            {
                CommonOpenFileDialog folderPicker = new CommonOpenFileDialog();
                folderPicker.InitialDirectory = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Packages\Microsoft.MinecraftUWP_8wekyb3d8bbwe\LocalState\games\com.mojang\minecraftWorlds");
                folderPicker.IsFolderPicker = true;
                if (folderPicker.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    dialog.FileName = folderPicker.FileName;
                    var result = await dialog.ShowAsync(ContentDialogPlacement.Popup);
                    if (result == ContentDialogResult.Primary)
                    {
                        Console.WriteLine("User options ready");
                        OnDatabaseOpen();
                    }
                }
            }
            else
            {
                OnDatabaseClose();
            }

        }

        private void tryParseKeyAndAdd(byte[] key)
        {
            int x = 0, z = 0, dimension = 0;
            byte flag = 0xff, subChunkIndex = 0xff;
            switch (key.Length)
            {
                case 9:
                    x = bytesToInt(key, 0);
                    z = bytesToInt(key, 4);
                    flag = key[8];
                    break;
                case 10:
                    x = bytesToInt(key, 0);
                    z = bytesToInt(key, 4);
                    flag = key[8];
                    subChunkIndex = key[9];
                    break;
                case 13:
                    x = bytesToInt(key, 0);
                    z = bytesToInt(key, 4);
                    dimension = bytesToInt(key, 8);
                    flag = key[12];
                    break;
                case 14:
                    x = bytesToInt(key, 0);
                    z = bytesToInt(key, 4);
                    dimension = bytesToInt(key, 8);
                    flag = key[12];
                    subChunkIndex = key[13];
                    break;
            }

            // Check for flags and index
            if (Enum.IsDefined(typeof(KeyType), flag))
            {
                var chunkPos = new ChunkPos(x, z, dimension);
                var keyType = (KeyType)flag;
                KeyItem chunkItem;
                chunks.TryGetValue(chunkPos, out chunkItem);
                if (chunkItem == null)
                {
                    if (dimension >= 0 && dimension <= 2)
                    {
                        chunks.Add(chunkPos,
                            chunkItem = new KeyItem
                            {
                                Name = "Chunk (" + x + ", " + z + ")",
                                Children = new ObservableCollection<KeyItem> { new KeyItem { Name = "SubChunks" } } // SubChunk is always the first item in Chunks
                            }
                        );
                        if (dimension == 0) overworldGroup.Children.Add(chunkItem);
                        else if (dimension == 1) netherGroup.Children.Add(chunkItem);
                        else if (dimension == 2) endGroup.Children.Add(chunkItem);
                    }
                }

                if (chunkItem != null)
                {
                    switch (keyType)
                    {
                        case KeyType.Chunk:
                            chunkItem.Data = key;
                            return;
                        case KeyType.SubChunk:
                            if (subChunkIndex >= 0 && subChunkIndex < 32)
                            {
                                var subChunkItem = chunkItem.Children[0];
                                subChunkItem.Children.Add(new KeyItem { Name = "SubChunk #" + subChunkIndex, Data = key });
                                return;
                            }
                            break;
                        default:
                            chunkItem.Children.Add(new KeyItem { Name = keyType.ToString(), Data = key });
                            return;
                    }
                }
            }

            generalKeys.Children.Add(new KeyItem { Name = asString(key), Data = key });
        }

        private int bytesToInt(byte[] input, int offset)
        {
            return input[offset] | input[offset + 1] << 8 | input[offset + 2] << 16 | input[offset + 3] << 24;
        }

        private String hexDump(byte[] input)
        {
            return BitConverter.ToString(input).Replace("-", "");
        }

        private String asString(byte[] input)
        {
            return Encoding.UTF8.GetString(input);
        }

        private void OnControlsSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Console.WriteLine("OnControlsSearchBoxQuerySubmitted");
            // TODO:
        }

        private void OnControlsSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            Console.WriteLine("OnControlsSearchBoxQuerySubmitted");
            // TODO:
        }

        void OnRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            UpdateTextBox();
        }

        private void OnTreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            var item = treeView.SelectedItem as KeyItem;
            if (item.Data != null)
            {
                UIntPtr length;
                currentKey = item.Data;
                currentValue = database.Get(currentKey, out length);
                Array.Resize(ref currentValue, (int)length);
            }
            else
            {
                currentKey = null;
                currentValue = null;
            }
            UpdateTextBox();
        }

        private void OnTreeViewItemRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        private void UpdateTextBox()
        {
            if (keyTextBox == null || valueTextBox == null)
            {
                return;
            }

            if (currentKey == null)
            {
                keyTextBox.Clear();
                valueTextBox.Clear();
                return;
            }

            switch (keyTypeRadioButtons.SelectedIndex)
            {
                case 0:
                    keyTextBox.Text = hexDump(currentKey);
                    break;
                case 1:
                    keyTextBox.Text = asString(currentKey);
                    break;
                default:
                    keyTextBox.Clear();
                    valueTextBox.Clear();
                    return;
            }

            switch (valueTypeRadioButtons.SelectedIndex)
            {
                case 0:
                    valueTextBox.Text = HexUtil.prettyHexDump(currentValue);
                    break;
                case 1:
                    valueTextBox.Text = asString(currentValue);
                    break;
                case 2:
                    try
                    {
                        using (var stream = new MemoryStream(currentValue))
                        {
                            fNbt.NbtReader reader = new fNbt.NbtReader(stream, false);
                            fNbt.NbtTag tag = reader.ReadAsTag();
                            valueTextBox.Text = tag.ToString("    ");
                        }
                    }
                    catch (Exception e)
                    {
                        valueTextBox.Text = e.ToString();
                    }
                    break;
                default:
                    keyTextBox.Clear();
                    valueTextBox.Clear();
                    return;
            }

        }
    }
}
