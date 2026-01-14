using System.Threading.Tasks;

namespace VisualDirector
{
    public interface IVisualDirectorInputProvider
    {
        Task InputDetected();
        Task ChoiceDetected();
    }
}
