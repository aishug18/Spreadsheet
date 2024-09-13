// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
// (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
// (Clarified meaning of dependent and dependee)
// (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024

namespace CS3500.DependencyGraph;

/// <summary>
///     <para>
///         (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
///         (in other words: s1 must be evaluated before t1.)
///     </para>
///     <para>
///         A DependencyGraph can be modeled as a set of ordered pairs of strings.
///         Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
///         if s1 equals s2 and t1 equals t2.
///     </para>
///     <remarks>
///         Recall that sets never contain duplicates.
///         If an attempt is made to add an element to a set, and the element is already
///         in the set, the set remains unchanged.
///     </remarks>
///     <para>
///         Given a DependencyGraph DG:
///     </para>
///     <list type="number">
///         <item>
///             If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///             (The set of things that depend on s.)
///         </item>
///         <item>
///             If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///             (The set of things that s depends on.)
///         </item>
///     </list>
///     <para>
///         For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}.
///     </para>
///     <code>
///         dependents("a") = {"b", "c"}
///         dependents("b") = {"d"}
///         dependents("c") = {}
///         dependents("d") = {"d"}
///         dependees("a") = {}
///         dependees("b") = {"a"}
///         dependees("c") = {"a"}
///         dependees("d") = {"b", "d"}
///     </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    ///     Stores the size of the Dependency Graph, i.e., the number of ordered pairs/dependencies in the graph.
    /// </summary>
    private int size;

    /// <summary>
    ///     This Dictionary stores the dependents of a node.
    ///     The dependents of a node are what depends on the node, i.e., what must be calculated AFTER this node.
    /// </summary>
    private Dictionary<string, HashSet<string>> dependents;

    /// <summary>
    ///     This Dictionary stores the dependees of a node.
    ///     The dependees of a node are what the node depends upon, i.e., what must be calculated BEFORE this node.
    /// </summary>
    private Dictionary<string, HashSet<string>> dependees;


    /// <summary>
    ///     Initializes a new instance of the <see cref="DependencyGraph"/> class.
    ///     The initial DependencyGraph is empty.
    /// </summary>
    public DependencyGraph()
    {
        dependents = new Dictionary<string, HashSet<string>>();
        dependees = new Dictionary<string, HashSet<string>>();
        size = 0;
    }

    /// <summary>
    ///     The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return size; }
    }

    /// <summary>
    ///     Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        if (dependents.TryGetValue(nodeName, out HashSet<string>? result) && result.Count > 0)
            return true;
        
        else
            return false;
    }

    /// <summary>
    ///     Reports whether the given node has dependees (i.e., depends on one or more other nodes).
    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        if (dependees.TryGetValue(nodeName, out HashSet<string>? result) && result.Count > 0)
            return true;

        else
            return false;
    }
    
    /// <summary>
    ///     <para>
    ///         Returns the dependents of the node with the given name.
    ///     </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (dependents.ContainsKey(nodeName))
            return dependents[nodeName];
        
        else
            return new HashSet<string>();
    }

    /// <summary>
    ///     <para>
    ///         Returns the dependees of the node with the given name.
    ///     </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if (dependees.ContainsKey(nodeName))
            return dependees[nodeName];

        else
            return new HashSet<string>();
    }

    /// <summary>
    ///     <para>
    ///         Adds the ordered pair (dependee, dependent), if it doesn't exist.
    ///     </para>
    ///     <para>
    ///         This can be thought of as: dependee must be evaluated before dependent
    ///     </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        // search for dependee in Dictionary dependents, then add dependent to dependee's hashset.
        if (dependents.TryGetValue(dependee, out HashSet<string>? result))
        {
            result.Add(dependent);
        }
        else
        {
            dependents.Add(dependee, new HashSet<string> { dependent });
        }

        // search for dependent in Dictionary dependees, then add dependee to dependent's hashset.
        if (dependees.TryGetValue(dependent, out HashSet<string>? result2))
        {
            result2.Add(dependee);
        }
        else // else if dependees does not contain dependent, add dependent with a new hashset that only contains dependee.
        {
            dependees.Add(dependent, new HashSet<string> { dependee });
        }

        size++;
    }

    /// <summary>
    ///     <para>
    ///         Removes the ordered pair (dependee, dependent), if it exists.
    ///     </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>
    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        // if dependee exists in Dependents AND dependent exists in dependees' hashset, then remove dependent from the hashset.
        // then remove dependee from dependent's hashset in Dependees.

        if (dependents.TryGetValue(dependee, out HashSet<string>? result) && result.Contains(dependent))
        {
            result.Remove(dependent);
            dependees[dependent].Remove(dependee);
        }
        else   // the ordered pair does not exist, so no change.
            return;

        size--;
    }

    /// <summary>
    ///     Removes all existing ordered pairs of the form (nodeName, *). Then, for each
    ///     t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced</param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        // update Dependents:
        // if nodeName exists in Dependents, then store the size of the existing hashset in a variable (oldSize = dependents[nodeName].Count), then store old hashset into a variable, then replace its hashset with newDependents --> dependents[nodeName] = newDependents
        // else, nodeName does NOT exist in Dependents, then dependents.add(nodeName, newDependents).
        
        int oldSize = 0;
        HashSet<string> oldDependents;

        if (dependents.TryGetValue(nodeName, out HashSet<string>? result))
        {
            oldSize = result.Count;
            oldDependents = new HashSet<string>(result);
            dependents[nodeName] = (HashSet<string>)newDependents;

            // removing nodeName from the dependees stuff
            foreach (string item in oldDependents)
            {
                if(dependees.TryGetValue(item, out HashSet<string>? result3))
                {
                    result3.Remove(nodeName);
                }
            }
        }
        else
            dependents.Add(nodeName, (HashSet<string>)newDependents);

        // update Dependees
        // for each item in newDependents, if item exists in Dependees, then dependees[item].add(nodeName).
        // if item does NOT exist in Dependees, then dependees.add(item, new HashSet<string>{nodeName}).

        foreach (string item in newDependents)
        {
            if(dependees.TryGetValue(item, out HashSet<string>? result2))
                result2.Add(nodeName);
            
            else
                dependees.Add(item, new HashSet<string> { nodeName });
        }

        // update size --> size = size - oldSize + newDependents.Count
        size = size - oldSize + newDependents.Count();

    }

    /// <summary>
    ///     <para>
    ///         Removes all existing ordered pairs of the form (*, nodeName). Then, for each
    ///         t in newDependees, adds the ordered pair (t, nodeName).
    ///     </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        // update Dependees:
        // if nodeName exists in Dependees, then store the size of the existing hashset in a variable (oldSize = dependees[nodeName].Count), then replace its hashset with newDependees --> dependees[nodeName] = newDependees
        // else, nodeName does NOT exist in Dependees, then dependees.add(nodeName, newDependees).

        int oldSize = 0;
        HashSet<string> oldDependees;

        if (dependees.TryGetValue(nodeName, out HashSet<string>? result))
        {
            oldSize = result.Count;
            oldDependees = new HashSet<string>(result);
            dependents[nodeName] = (HashSet<string>)newDependees;

            // removing nodeName from the dependents stuff
            foreach (string item in oldDependees)
            {
                if (dependents.TryGetValue(item, out HashSet<string>? result3))
                {
                    result3.Remove(nodeName);
                }
            }
        }
        else
            dependees.Add(nodeName, (HashSet<string>)newDependees);

        // update Dependents
        // for each item in newDependees, if item exists in Dependents, then dependents[item].add(nodeName).
        // if item does NOT exist in Dependents, then dependents.add(item, new HashSet<string>{nodeName}).

        foreach (string item in newDependees)
        {
            if (dependents.TryGetValue(item, out HashSet<string>? result2))
                result2.Add(nodeName);

            else
                dependents.Add(item, new HashSet<string> { nodeName });
        }

        // update size --> size = size - oldSize + newDependees.Count
        size = size - oldSize + newDependees.Count();
    }
}