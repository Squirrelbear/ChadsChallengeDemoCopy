using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameDevMajor
{
    public class DependencyCollection
    {
        private List<BlockDependency> dependencies;

        private Level lvl;

        public DependencyCollection(Level lvl)
        {
            this.lvl = lvl;
            dependencies = new List<BlockDependency>();
        }

        public void checkDependencies(Vector2 pos)
        {
            foreach(BlockDependency dependency in dependencies)
            {
                dependency.checkRequireAction(pos);
            }
        }

        public void removeDependency(BlockDependency dependency)
        {
            dependencies.Remove(dependency);
        }

        public void addDependency(BlockDependency dependency)
        {
            dependencies.Add(dependency);
        }
    }
}
