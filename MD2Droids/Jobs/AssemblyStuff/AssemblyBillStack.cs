using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MD2
{
    public class AssemblyBillStack : IExposable
    {
        private AssemblyStation _assembly;
        private List<AssemblyBill> _bills = new List<AssemblyBill>();

        public AssemblyBillStack(AssemblyStation assembly)
        {
            _assembly = assembly;
        }

        public bool HasBill
        {
            get { return _bills.Count > 0; }
        }

        public AssemblyBill CurrentBill
        {
            get { return _bills[0]; }
        }

        public AssemblyStation Assembly
        {
            get { return _assembly; }
        }

        public int Count
        {
            get { return _bills.Count; }
        }

        public List<AssemblyBill> Bills
        {
            get { return _bills; }
        }

        public IEnumerator<AssemblyBill> GetEnumerator()
        {
            return _bills.GetEnumerator();
        }

        public void AddBill(AssemblyBill bill)
        {
            _bills.Add(bill);
        }

        private void FinishBill(AssemblyBill bill)
        { }

        private void DeleteBill(AssemblyBill bill)
        { }

        public void ExposeData()
        {
            Scribe_Collections.LookList(ref _bills, "bills", LookMode.Deep);
        }
    }
}
