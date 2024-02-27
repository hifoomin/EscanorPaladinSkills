using System;
using System.Collections.Generic;
using System.Text;

namespace EscanorPaladinSkills.SkillDefs
{
    public abstract class SkillDefBase<T> : SkillDefBase where T : SkillDefBase<T>
    {
        public static T instance { get; set; }

        public SkillDefBase()
        {
            if (instance != null)
            {
                throw new InvalidOperationException("Singleton class " + typeof(T).Name + " was instantiated twice");
            }
            instance = this as T;
        }
    }
}