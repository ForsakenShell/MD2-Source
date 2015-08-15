using System.Text;

namespace MD2
{
    public static class TicksToTime
    {

        public static string GetTime(float ticks)
        {
            int seconds = (int)(ticks / 60f);
            int minutes = seconds / 60;
            int hours = minutes / 60;

            if (seconds > 59)
                seconds = seconds % 60;
            if (minutes > 59)
                minutes = minutes % 60;

            StringBuilder str = new StringBuilder();
            if (hours < 10)
            {
                str.Append(0 + hours.ToString() + ":");
            }
            else
                str.Append(hours.ToString() + ":");
            if (minutes < 10)
            {
                str.Append(0 + minutes.ToString() + ":");
            }
            else
                str.Append(minutes.ToString() + ":");
            if (seconds < 10)
            {
                str.Append(0 + seconds.ToString());
            }
            else
                str.Append(seconds.ToString());
            return str.ToString();
        }
    }
}
