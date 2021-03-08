using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlistosu
{
    class MapInfo
    {
        public byte version;

        #region General
        public string audioFilename;
        #endregion

        #region Metadata
        public string title, titleUnicode;
        public string artist, artistUnicode;
        public string mapCreator; // For later use in selecting map to add to playlist
        public string source;
        public string mapID, mapsetID;
        #endregion

        #region Not in osu! map file
        public uint length;
        #endregion

        public bool IsMissingMetadata()
        {
            if (title == null ||
                titleUnicode == null ||
                artist == null ||
                artistUnicode == null)
            {
                return true;
            }
            else return false;
        }
    }
}