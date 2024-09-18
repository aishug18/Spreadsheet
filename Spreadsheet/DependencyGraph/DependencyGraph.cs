// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
// (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
// (Clarified meaning of dependent and dependee)
// (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024
// Final implementation by Aishwarya Golakoti, 13 September 2024.

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
        if (dependents.TryGetValue(nodeName, out HashSet<string>? result))
            return result;
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
        if (dependees.TryGetValue(nodeName, out HashSet<string>? result))
            return result;
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
        // update dependents
        if (!dependents.ContainsKey(dependee))
        {
            dependents.Add(dependee, new HashSet<string> { dependent });
            size++;
        }
        else if (dependents[dependee].Add(dependent))
            size++;
        else
            return; // the ordered pair already exists, so exit.

        // update dependees
        if (dependees.TryGetValue(dependent, out HashSet<string>? result))
            result.Add(dependee);
        else
            dependees.Add(dependent, new HashSet<string> { dependee });
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
        if (dependents.TryGetValue(dependee, out HashSet<string>? result) && result.Contains(dependent))
        {
            result.Remove(dependent);
            dependees[dependent].Remove(dependee);
        }
        else
            return; // the ordered pair does not exist, so exit.

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
        int oldSize = 0;
        HashSet<string> oldDependents;
        HashSet<string> newDeps = new HashSet<string>(newDependents);

        // update Dependents
        if (dependents.TryGetValue(nodeName, out HashSet<string>? result))
        {
            oldSize = result.Count;
            oldDependents = new HashSet<string>(result);
            dependents[nodeName] = newDeps;

            foreach (string item in oldDependents)
            {
                if (dependees.TryGetValue(item, out HashSet<string>? result2))
                    result2.Remove(nodeName);
            }
        }
        else
        {
            
            dependents.Add(nodeName, newDeps);
        }

        // update Dependees
        foreach (string item in newDependents)
        {
            if(dependees.TryGetValue(item, out HashSet<string>? result3))
                result3.Add(nodeName);
            else
                dependees.Add(item, new HashSet<string> { nodeName });
        }

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
        int oldSize = 0;
        HashSet<string> oldDependees;
        HashSet<string> newDees = new HashSet<string>(newDependees);

        // update Dependees
        if (dependees.TryGetValue(nodeName, out HashSet<string>? result))
        {
            oldSize = result.Count;
            oldDependees = new HashSet<string>(result);
            dependees[nodeName] = newDees;

            foreach (string item in oldDependees)
            {
                if (dependents.TryGetValue(item, out HashSet<string>? result2))
                    result2.Remove(nodeName);
            }
        }
        else
            dependees.Add(nodeName, newDees);

        // update Dependents
        foreach (string item in newDependees)
        {
            if (dependents.TryGetValue(item, out HashSet<string>? result3))
                result3.Add(nodeName);
            else
                dependents.Add(item, new HashSet<string> { nodeName });
        }

        size = size - oldSize + newDependees.Count();
    }

}