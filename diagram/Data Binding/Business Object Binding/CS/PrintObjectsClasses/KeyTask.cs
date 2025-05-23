#region Copyright Syncfusion® Inc. 2001-2025.
// Copyright Syncfusion® Inc. 2001-2025. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;

using System.Web;


    [Serializable]
    public class KeyTask
    {
        private string m_desc;
        private string m_leadr;
        private decimal m_cst;
        public string Description { get { return m_desc; } set { m_desc = value; } }
        public string Leader { get { return m_leadr; } set { m_leadr = value; } }
        public decimal CostSavings { get { return m_cst; } set { m_cst = value; } }
       
    }
