namespace CS3500.DevelopmentTests;

using CS3500.DependencyGraph;

/// <summary>
///     This is a test class for DependencyGraphTest and is intended
///     to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    ///     TODO: Explain carefully what this code tests.
    ///     Also, update in-line comments as appropriate.
    /// </summary>
    [TestMethod]
    [Timeout(2000)] // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    // ---- Tests for Size ----

    [TestMethod]
    public void TestSize_EmptyGraph_IsZero()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.AreEqual(0, dg.Size);
    }

    [TestMethod]
    public void TestSize_FilledGraph_CorrectValue()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.AreEqual(4, dg.Size);
    }

    // ---- Tests for HasDependents ----

    [TestMethod]
    public void TestHasDependents_EmptyGraph_False()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.IsFalse(dg.HasDependents("a1"));
    }

    [TestMethod]
    public void TestHasDependents_NodeDNE_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.IsFalse(dg.HasDependents("b1"));
    }

    [TestMethod]
    public void TestHasDependents_NoDependents_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.IsFalse(dg.HasDependents("c5"));

    }

    [TestMethod]
    public void TestHasDependents_HasDependents_True()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.IsTrue(dg.HasDependents("a2"));
    }

    // ---- Tests for HasDependees ----

    [TestMethod]
    public void TestHasDependees_EmptyGraph_False()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.IsFalse(dg.HasDependents("b1"));
    }

    [TestMethod]
    public void TestHasDependees_NodeDNE_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.IsFalse(dg.HasDependees("b1"));
    }

    [TestMethod]
    public void TestHasDependees_NoDependees_False()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.IsFalse(dg.HasDependees("a1"));
    }

    [TestMethod]
    public void TestHasDependees_HasDependees_True()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        Assert.IsTrue(dg.HasDependees("b4"));
    }

    // ---- Tests for GetDependents ----

    [TestMethod]
    public void TestGetDependents_EmptyGraph_EmptySet()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> expected = new HashSet<string> {};
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("a1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestGetDependents_NodeDNE_EmptySet()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        HashSet<string> expected = new HashSet<string> {};
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("h1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestGetDependents_NoDependents_EmptySet()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        HashSet<string> expected = new HashSet<string> {};
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("c5");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestGetDependents_HasDependents_ValidSet()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        HashSet<string> expected = new HashSet<string> { "a2", "a3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("a1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    // ---- Tests for GetDependees ----

    [TestMethod]
    public void TestGetDependees_EmptyGraph_EmptySet()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> expected = new HashSet<string> {};
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("a1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestGetDependees_NodeDNE_EmptySet()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        HashSet<string> expected = new HashSet<string> {};
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("m1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestGetDependees_NoDependees_EmptySet()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        HashSet<string> expected = new HashSet<string> {};
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("a1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestGetDependees_HasDependees_ValidSet()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");
        HashSet<string> expected = new HashSet<string> { "a1"};
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("a3");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    // ---- Tests for AddDependency ----

    [TestMethod]
    public void TestAddDepedency_UniquePair_AddsCorrectly()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        Assert.AreEqual(2, dg.Size);
        Assert.IsTrue(dg.HasDependents("a1"));
        Assert.IsTrue(dg.HasDependees("a3"));
    }

    public void TestAddDependency_DuplicatePair_NoChange()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a1", "a3");
        Assert.AreEqual(2, dg.Size);
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("a1");
        Assert.AreEqual(2, actual.Count);
    }

    // ---- Tests for RemoveDependency ----

    [TestMethod]
    public void TestRemoveDepedency_EmptyGraph_NoChange()
    {
        DependencyGraph dg = new DependencyGraph();
        Assert.AreEqual(0, dg.Size);
        dg.RemoveDependency("a3", "a4");
        Assert.AreEqual(0, dg.Size);
    }

    [TestMethod]
    public void TestRemoveDepedency_DependencyExists_RemoveCorrectly()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.RemoveDependency("a1", "a2");
        Assert.AreEqual(1, dg.Size);
        Assert.IsFalse(dg.HasDependees("a2"));
    }

    public void TestRemoveDependency_DependencyDNE_NoChange()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.RemoveDependency("b1", "a2");
        Assert.AreEqual(2, dg.Size);
    }

    // ---- Tests for ReplaceDependents ----

    [TestMethod]
    public void TestReplaceDepedents_EmptyGraph_StillReplacesDependents()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> newDependents = new HashSet<string> { "h1", "h2", "h3" };
        dg.ReplaceDependents("a1", newDependents);

        Assert.AreEqual(3, dg.Size);

        HashSet<string> expected = new HashSet<string> { "h1", "h2", "h3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("a1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestReplaceDependents_NodeDNE_StillReplacesDependents()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");

        HashSet<string> newDependents = new HashSet<string> { "h1", "h2", "h3" };
        dg.ReplaceDependents("p1", newDependents);

        Assert.AreEqual(7, dg.Size);

        HashSet<string> expected = new HashSet<string> { "h1", "h2", "h3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("p1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    public void TestReplaceDependents_NodeExists_ReplacesCorrectly()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");

        HashSet<string> newDependents = new HashSet<string> { "h1", "h2", "h3" };
        dg.ReplaceDependents("a1", newDependents);

        Assert.AreEqual(5, dg.Size);

        HashSet<string> expected = new HashSet<string> { "h1", "h2", "h3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependents("a1");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    // ---- Tests for ReplaceDependees ----

    [TestMethod]
    public void TestReplaceDependees_EmptyGraph_StillReplacesDependees()
    {
        DependencyGraph dg = new DependencyGraph();
        HashSet<string> newDependees = new HashSet<string> { "h1", "h2", "h3" };
        dg.ReplaceDependees("a3", newDependees);

        Assert.AreEqual(3, dg.Size);

        HashSet<string> expected = new HashSet<string> { "h1", "h2", "h3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("a3");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    [TestMethod]
    public void TestReplaceDependees_NodeDNE_StillReplacesDependees()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");

        HashSet<string> newDependees = new HashSet<string> { "h1", "h2", "h3" };
        dg.ReplaceDependees("p3", newDependees);

        Assert.AreEqual(7, dg.Size);

        HashSet<string> expected = new HashSet<string> { "h1", "h2", "h3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("p3");
        Assert.IsTrue(expected.SetEquals(actual));
    }

    public void TestReplaceDependees_NodeExists_ReplacesCorrectly()
    {
        DependencyGraph dg = new DependencyGraph();
        dg.AddDependency("a1", "a2");
        dg.AddDependency("a1", "a3");
        dg.AddDependency("a2", "b4");
        dg.AddDependency("a3", "c5");

        HashSet<string> newDependees = new HashSet<string> { "h1", "h2", "h3" };
        dg.ReplaceDependees("a3", newDependees);

        Assert.AreEqual(6, dg.Size);

        HashSet<string> expected = new HashSet<string> { "h1", "h2", "h3" };
        HashSet<string> actual = (HashSet<string>)dg.GetDependees("a3");
        Assert.IsTrue(expected.SetEquals(actual));
    }

}