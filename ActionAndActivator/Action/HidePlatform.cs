namespace Assets.ActionAndActivator
{
	public class HidePlatform : ActionBase
	{
		public override void Execute(ActivatorBase activator)
		{
			if(activator.IsSignal)
			{
				gameObject.SetActive(false);
			}
			else
			{
				gameObject.SetActive(true);
			}
		}
	}
}
