using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Esmart.Framework.Patterns.Publisher
{
    /// <summary>
    /// first level navigator enum
    /// </summary>
    public enum Subject
    {
        Login_Success = 1,
        Logout_Sucess,
        Move_Forward,
        Move_Backward,
        Goto_System,
        Goto_Edit,
        A_Cavity,
        B_Cavity,
        C_Cavity,
        D_Cavity,
        SendSaveSuccessMessage,
        SendErroMessage,
        SendWarningMessage,
        //Goto_SystemMonitor,
        //Goto_ChamberMonitor,
        //Goto_RecipeEditor,
        //Goto_SequenceEditor,
        SwitchView
        // Add your subjects here.
    }
}
