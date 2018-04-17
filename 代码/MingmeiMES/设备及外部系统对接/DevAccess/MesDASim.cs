using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevInterface;
namespace DevAccess
{
    public class MesDASim:IMesAccess
    {
        public string MesdbConnstr { get; set; }
        public bool ConnDB(ref string reStr) { return true; }
        public bool DisconnDB(ref string reStr) { return true; }
        public int MesAssemAuto(string[] paramArray, ref string reStr) { return 0; }
        public int MesAssemDown(string[] paramArray, ref string reStr) { return 0; }
        public int MesReAssemEnabled(string[] paramArray, ref string reStr) { return 0; }
        public int MesDownEnabled(string mesLinID, string barcode, string downQueryMesID, ref string reStr) { return 0; }
    }
}
