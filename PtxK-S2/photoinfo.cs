using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PtxK_S2
{
    [DataContract]
    public class photoinfo
    {
        [DataMember]
		public int errCode { get; set; }
		[DataMember]
		public string errMsg { get; set; }

		[DataMember]
        public bool captured {get; set;}
		[DataMember]
        public string dir {get; set;}
		[DataMember]
        public string file {get; set;}
		[DataMember]
        public string av { get; set; }
		[DataMember]
        public string sv { get; set; }
		[DataMember]
        public string xv { get; set; }
		[DataMember]
        public string tv { get; set; }
		[DataMember]
        public string orientation { get; set; }
        [DataMember]
        public string cameraModel { get; set; }
        [DataMember]
        public string latlng { get; set; }
        [DataMember]
        public string datetime { get; set; }
    }
}
