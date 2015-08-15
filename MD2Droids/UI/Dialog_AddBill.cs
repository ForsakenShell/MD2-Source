using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class Dialog_AddBill : Layer_Window
    {
        private string _fileName;
        private AssemblyStation _assembly;
        private string _name = "";
        private const int MaxNameLength = 28;
        public Dialog_AddBill(AssemblyStation assembly, string fileName)
        {
            _assembly = assembly;
            _fileName = fileName;
            _name = ListerDroids.GetNumberedName();
            base.SetCentered(280f, 175f);
            this.drawPriority = 2000;
            this.closeOnEscapeKey = true;
            this.doCloseX = true;
            this.absorbAllInput = true;
            this.clearNonEditWindows = false;
            this.forcePause = true;
        }

        protected override void FillWindow(Rect inRect)
        {
            Text.Font = GameFont.Small;
            GUI.BeginGroup(inRect);
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }
            string text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), this._name);
            if (text.Length < MaxNameLength)
            {
                this._name = text;
            }
            if (Widgets.TextButton(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK") || flag)
            {
                Blueprint bp = BlueprintFiles.LoadFromFile(_fileName);
                bp.Name = _name;
                _assembly.AssemblyBillStack.AddBill(new AssemblyBill(_assembly.AssemblyBillStack,bp));
                Close();
            }
            GUI.EndGroup();
        }
    }
}
