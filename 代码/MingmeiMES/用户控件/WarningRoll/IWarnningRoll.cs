using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarningRoll
{
    public interface IWarnningRoll
    {
        void SetWarnningStr(string warnStr);

        void SetFontSize(int fontSize);

        void SetFontColor(Brush color);

        void SetSpeed(int speed);
        void StartRoll();
        void StopRoll();

    }
}
