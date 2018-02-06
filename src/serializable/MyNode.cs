using System;

namespace Demo
{
    [Serializable]
    public class MyNode
    {
        public string NodeText { get; set; }
        public string NodeId { get; set; }
        public string ParentId { get; set; }
        public string TimeStamp { get; set; }
    }
}