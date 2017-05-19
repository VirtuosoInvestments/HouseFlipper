using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hack.HouseFlipper.Utility
{    
    public class RuleSet : List<IRule>, IRule
    {
        public RuleSet(RuleCondition condition)
        {
            this.Condition = condition;
        }

        public RuleCondition Condition { get; set; }

        public bool IsSatisfied(params object[] variables)
        {
            throw new NotImplementedException();
        }
    }
}
