using System;

[Serializable]
public class MatchMakingSettings
{
	public virtual bool IsValidMatchFor(MatchMakingSettings matchMakingData)
	{
		return true;
	}
}