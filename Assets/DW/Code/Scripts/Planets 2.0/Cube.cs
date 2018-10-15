using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace dw.planet {
	public class Cube  {
        #region Variables
        public readonly Face[] faces = new Face[6];
        #endregion;

        #region Constructor
        public Cube(Face[] faces)
        {
            this.faces = faces;
        }
		#endregion;

		#region Unity Methods
		void Start () {
			
		}
	
		void Update () {
			
		}
		#endregion;
	}
}