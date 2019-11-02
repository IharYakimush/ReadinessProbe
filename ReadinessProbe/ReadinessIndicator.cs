using System;

namespace ReadinessProbe
{
    public abstract class ReadinessIndicator
    {
        public bool IsReady { get; protected set; }

        public void Ready()
        {
            if (this.IsReady)
            {
                throw new InvalidOperationException();
            }

            this.IsReady = true;
        }
    }
}