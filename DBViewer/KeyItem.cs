using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBViewer
{
    public enum KeyType : byte
    {
        ChunkVersion = 0x2c,
        Chunk = 0x2d,
        //Data2DLegacy = 0x2e,
        SubChunk = 0x2f,
        //TerrainLegacy = 0x30,
        BlockEntity = 0x31,
        Entity = 0x32,
        PendingTicks = 0x33,
        BlockExtraData = 0x34,
        BiomeState = 0x35,
        FinalizedState = 0x36,
        BorderBlocks = 0x38,
        HardCodedSpawnAreas = 0x39,
        RandomTicks = 0x3a,
        Checksums = 0x3b,
        //ChunkVersionLegacy = 0x3c
    }

    public class KeyItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Name
        public String Name { get; set; }

        public byte[] Data { get; set; }

        // Children
        private ObservableCollection<KeyItem> m_children;
        public ObservableCollection<KeyItem> Children
        {
            get
            {
                if (m_children == null)
                {
                    m_children = new ObservableCollection<KeyItem>();
                }
                return m_children;
            }
            set
            {
                m_children = value;
            }
        }

        private bool m_isExpanded;
        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        private bool m_isSelected;
        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
