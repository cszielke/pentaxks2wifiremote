using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PtxK_S2
{
	[DataContract]
    public class shoot
    {
		[DataMember]
		public int errCode { get; set; }
		[DataMember]
		public string errMsg { get; set; }
		[DataMember]
		public bool focused { get; set; }
		[DataMember]
		public List<string> focusCenters { get; set; }
		[DataMember]
		public bool captured { get; set; }

    }
}

//Beispiel:
//{"errCode": 200,
// "errMsg": "OK",
// "focused": false,
// "focusCenters": [],
// "captured": false}