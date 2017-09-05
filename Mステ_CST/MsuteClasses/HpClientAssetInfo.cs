using System;

namespace MsuteClasses
{
    public class HpClientAssetInfo : HpMasterData
    {
        public int id;

        public string name = string.Empty;

        public string hash = string.Empty;

        public int filesize;

        public override string ToString()
        {
            return string.Concat(new object[]
		{
			"[HpClientAssetInfo] id=",
			this.id.ToString(),
			", name=",
			this.name.ToString(),
			", hash=",
			this.hash.ToString(),
			", filesize=",
			this.filesize
		});
        }
    }
}