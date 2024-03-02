namespace Jega.Utils
{
    public interface IService
    {
        public void Preprocess();
        public void Postprocess();
        public int Priority { get; }
    }
}