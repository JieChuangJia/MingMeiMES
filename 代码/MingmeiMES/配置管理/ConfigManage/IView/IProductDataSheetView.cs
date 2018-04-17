using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
namespace ConfigManage
{
    public interface IProductDataSheetView
    {
        void DispHeightDeflist(DataTable dt);
        void DispPacksizeDeflist(DataTable dt);
        void DispProductCfgList(DataTable dt);
        void ShowPopupMes(string mes);
        int AskMessge(string mes);
        void RefreshHeightList(List<string> heightList);
        void RefreshPacksizeList(List<string> packsizeList);
        void RefreshGasList(List<string> gasList);

    }
}
