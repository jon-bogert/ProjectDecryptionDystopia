namespace XephTools
{
    public static class XephMath
    {
        public static float Remap(int input, int inMin, int inMax, float outMin, float outMax)
        {
            return (input - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
    }
}
