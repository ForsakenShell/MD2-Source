using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class OrderStack : IExposable
    {
        private AssemblyLine line;
        private List<Order> orders = new List<Order>();
        private const int maxOrders = 10;

        public OrderStack(AssemblyLine line)
        {
            this.line = line;
        }

        public Order this[int index]
        {
            get
            {
                return this.orders[index];
            }
        }

        public bool Any(Func<Order,bool> predicate)
        {
            return this.orders.Any(predicate);
        }

        public bool Contains(Order order)
        {
            return this.orders.Contains(order);
        }

        public void Reorder(Order order, MoveDir dir)
        {
            if (dir == MoveDir.Up)
            {
                if (this.Count > 1 && this.Contains(order) && order.CanMoveUp)
                {
                    int num = this.IndexOf(order) - 1;
                    this.Remove(order);
                    this.Insert(num, order);
                    return;
                }
            }
            else if (dir == MoveDir.Down)
            {
                if (this.Count > 1 && this.Contains(order) && order.CanMoveDown)
                {
                    int num = this.IndexOf(order) + 1;
                    this.Remove(order);
                    this.Insert(num, order);
                    return;
                }
            }
            throw new InvalidOperationException();
        }

        public void ToBottom(Order order)
        {
            this.orders.Remove(order);
            this.orders.Add(order);
        }
        public void ToTop(Order order)
        {
            this.orders.Remove(order);
            this.orders.Insert(0, order);
        }

        public void ExposeData()
        {
            Scribe_Collections.LookList<Order>(ref this.orders, "orders", LookMode.Deep,this.line);
        }

        public Order CurrentOrder
        {
            get
            {
                if (this.orders.Count > 0)
                {
                    return this.orders[0];
                }
                else
                    return null;
            }
        }

        public void FinishOrderAndGetNext(Order order, bool delete = false)
        {
            if (!delete)
            {
                ToBottom(order);
            }
            else
            {
                Remove(order);
            }
        }


        public void AddNewOrder(Order order, bool atTop = false)
        {
            if (atTop)
            {
                this.Insert(0, order);
            }
            else
            {
                this.Add(order);
            }
        }

        public bool CanAcceptMoreOrders
        {
            get
            {
                return this.orders.Count < maxOrders;
            }
        }

        public int Count
        {
            get
            {
                return this.orders.Count;
            }
        }

        public IEnumerable<Order> All
        {
            get
            {
                foreach (var x in this.orders)
                    yield return x;
            }
        }

        public int IndexOf(Order order)
        {
            return this.orders.IndexOf(order);
        }

        public IEnumerator<Order> GetEnumerator()
        {
            return this.orders.GetEnumerator();
        }

        public void Remove(Order order)
        {
            this.orders.Remove(order);
        }

        public void Add(Order order, bool atTop = false)
        {
            if (atTop)
            {
                this.orders.Insert(0, order);
            }
            else
            {
                this.orders.Add(order);
            }
        }

        public void Insert(int index, Order order)
        {
            if (index >= 0)
            {
                this.orders.Insert(index, order);
            }
        }
    }
}
