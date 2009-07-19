namespace Radegast
{
    public interface IRadegastPlugin
    {
        void StartPlugin(RadegastInstance inst);
        void StopPlugin(RadegastInstance inst);
    }
}