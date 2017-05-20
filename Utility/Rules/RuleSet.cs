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

        public RuleSet(RuleCondition condition, params IRule[] rules)
        {
            this.Condition = condition;
            if(rules!=null)
            {
                foreach(var r in rules)
                {
                    this.Add(r);
                }
            }
        }

        public RuleCondition Condition { get; set; }

        public object Results
        {
            get
            {
                return this as List<IRule>;
            }
        }

        public bool IsSatisfied(params object[] variables)
        {
            switch(this.Condition)
            {
                case RuleCondition.OR:
                    foreach(var r in this)
                    {
                        if(r.IsSatisfied(variables))
                        {
                            return true;
                        }
                    }
                    return false;
                case RuleCondition.AND:                    
                    foreach (var r in this)
                    {
                        if (!r.IsSatisfied(variables))
                        {
                            return false;        
                        }
                    }
                    return true;
                default:
                    throw new NotImplementedException("Unhandled condition - " + this.Condition);
            }
            
        }
    }
}
