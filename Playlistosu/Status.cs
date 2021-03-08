using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Playlistosu
{
    enum Status
    {
        Done = 0,
        MissingMetadata = 1,
        NoAudioPath = 2,
        NotSupported = 3
    }
}
