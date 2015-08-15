using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace MD2
{
    public class Cart_Pather : IExposable //Saveable
    {
        private Cart cart;
        private Direction direction = Direction.Up.Random();
        private Track destination = null;
        private Track previousTrack = null;
        private int ticksUntilMove = 0;
        private int totalMoveDuration = 1;
        protected bool moving = false;
        protected bool stopped = false;

        public Cart_Pather(Cart cart)
        {
            this.cart = cart;
        }

        public bool Moving
        {
            get
            {
                return moving;
            }
        }

        public Track Destination
        {
            get
            {
                return destination;
            }
        }

        public Direction Direction
        {
            get
            {
                return this.direction;
            }
        }

        public bool Stopped
        {
            get
            {
                return this.stopped;
            }
        }

        public int TicksUntilMove
        {
            get
            {
                return this.ticksUntilMove;
            }
            set
            {
                this.ticksUntilMove = value;
            }
        }

        public int TotalMoveDuration
        {
            get
            {
                return this.totalMoveDuration;
            }
        }

        public Track CurTrack
        {
            get
            {
                return Find.ThingGrid.ThingAt<Track>(cart.Position);
            }
        }

        public bool DestinationIsValid
        {
            get
            {
                bool flag = true;

                if (destination == null)
                    flag = false;

                if (destination.Destroyed)
                    flag = false;

                if (!destination.IsAdjacentTo(CurTrack))
                    flag = false;

                if (!(destination.Position.InBounds()))
                    flag = false;

                if (!destination.CanBeTravelledTo)
                    flag = false;

                return flag;
            }
        }

        public virtual void Tick()
        {
            if (CurTrack == null || Stopped)
            {
                return;
            }
            if (!Moving)
            {
                StartNewMove();
            }
            else
            {
                MovementTick();
            }
        }

        public virtual void ToggleStopped()
        {
            if (stopped)
                stopped = false;
            else
            {
                stopped = true;
                StopDead();
            }
        }

        public virtual void StopDead()
        {
            moving = false;
            ticksUntilMove = 1;
            totalMoveDuration = 1;
            destination = null;
            previousTrack = null;
        }

        private void MoveTo(Track track)
        {
            previousTrack = CurTrack;
            cart.Position = track.Position;
            moving = false;
            ticksUntilMove = 0;
        }

        private void MovementTick()
        {
            if (DestinationIsValid)
            {
                if (TicksUntilMove > 0)
                {
                    TicksUntilMove--;
                    return;
                }
                if (Moving)
                {
                    MoveTo(destination);
                }
            }
            else
            {
                StopDead();
            }
        }

        private void StartNewMove()
        {
            if (CurTrack.HasValidPath && !CurTrack.ShouldStopCart)
            {
                destination = CurTrack.NextTrack(direction, previousTrack);
                if (destination != null && destination.CanBeTravelledTo)
                {
                    SetupNewMove();
                    previousTrack = CurTrack;
                    direction = CurTrack.DirectionTo(destination);
                    if (Direction != Direction.Any && Direction != Direction.Invalid)
                    {
                        moving = true;
                    }
                }
            }
        }

        private void SetupNewMove()
        {
            int ticks = cart.TicksPerMove();
            if (ticks > 450)
            {
                ticks = 450;
            }
            totalMoveDuration = ticks;
            ticksUntilMove = ticks;
        }

        public void ExposeData()
        {
            Scribe_Values.LookValue(ref this.stopped, "stoppped", false);
            Scribe_Values.LookValue(ref this.direction, "direction", Direction.Any);
            Scribe_References.LookReference(ref this.previousTrack, "previousTrack");
        }
    }
}
