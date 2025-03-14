using System.Collections;

namespace FPV
{
    /// <summary>
    /// Main controller for the  <see cref="PlayerApplication"></see>
    /// </summary>
    public class PlayerController : Controller<PlayerApplication>
    {
        internal override void RemoveListeners()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator WaitFor5Secomds()
        {
            yield return BetterCoroutines.FiveSeconds;
            
            
        }
    }
}