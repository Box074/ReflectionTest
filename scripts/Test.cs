
using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Concurrent;
using Modding;
using Satchel;
using Vasi;
using HKTool;
using HKTool.Reflection;
using MonoMod.Utils;
using System.Reflection.Emit;
using static HKTool.Utils.Compile.ReflectionHelperEx;
using UnityEngine;

public class ReflectionTest : ModBase<ReflectionTest>
{
    public const int RUN_COUNT = 1000000;
    public void Test(string name, Action action)
    {
        Stopwatch watch = new();

        watch.Start();
        action();
        watch.Stop();

        Log($"Time Spent({name}): {watch.ElapsedMilliseconds}");
        Log($"Average Time({name}): {watch.ElapsedMilliseconds / (double)RUN_COUNT}");
    }
    private int TestCount = 0;
    public ReflectionTest()
    {
        Test("Direct", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                TestCount = TestCount + 1;
            }
        });
        #region Reflected through an existing FieldInfo instance (ms)
        Log("Reflected through an existing FieldInfo instance (ms)");
        FieldInfo field = typeof(ReflectionTest).GetField("TestCount", HKTool.Reflection.ReflectionHelper.All);
        #region HKTool
        Test("HKToolRefHelper", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                GetFieldRefFrom<int>(GetFieldRefPointer(this, field)) = GetFieldRefFrom<int>(GetFieldRefPointer(this, field)) + 1;
            }
        });
        Test("HKToolReflectionHelper", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                field.FastSet(this, (int)field.FastGet(this)! + 1);
            }
        });
        #endregion
        #region Vasi
        Test("Vasi", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                Mirror.SetField(this, field, Mirror.GetField<ReflectionTest, int>(this, field) + 1);
            }
        });
        #endregion
        #region System.Reflection
        Test("System.Reflection", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                field.SetValue(this, ((int)field.GetValue(this) + 1));
            }
        });
        #endregion
        #endregion
        #region Reflect without passing an existing FieldInfo instance
        Log("Reflect without passing an existing FieldInfo instance");
        #region HKTool
        Test("HKToolRefHelper", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                GetFieldRefFrom<int>(GetFieldRefPointer(this, typeof(ReflectionTest).GetField("TestCount", HKTool.Reflection.ReflectionHelper.All))) = GetFieldRefFrom<int>(GetFieldRefPointer(this, typeof(ReflectionTest).GetField("TestCount", HKTool.Reflection.ReflectionHelper.All))) + 1;
            }
        });
        Test("HKToolRefHelper(Tool Generation)", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                this.private_TestCount() = this.private_TestCount() + 1;
            }
        });
        Test("HKToolReflectionObject", () =>
        {
            ReflectionObject obj = this.CreateReflectionObject();
            for (int i = 0; i < RUN_COUNT; i++)
            {
                obj.SetMemberData<int>("TestCount", obj.GetMemberData<int>("TestCount") + 1);
            }
        });
        Test("HKToolReflectionHelper", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                typeof(ReflectionTest).GetField("TestCount", HKTool.Reflection.ReflectionHelper.All).FastSet(this, (int)field.FastGet(this)! + 1);
            }
        });
        #endregion
        #region MAPI
        Test("ModdingAPI", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                Modding.ReflectionHelper.SetField(this, "TestCount", Modding.ReflectionHelper.GetField<ReflectionTest, int>(this, "TestCount"));
            }
        });
        #endregion
        #region Vasi
        Test("Vasi", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                Mirror.SetField(this, "TestCount", Mirror.GetField<ReflectionTest, int>(this, "TestCount") + 1);
            }
        });
        #endregion
        #region System.Reflection
        Test("System.Reflection", () =>
        {
            for (int i = 0; i < RUN_COUNT; i++)
            {
                typeof(ReflectionTest).GetField("TestCount", HKTool.Reflection.ReflectionHelper.All).SetValue(this, ((int)typeof(ReflectionTest).GetField("TestCount", HKTool.Reflection.ReflectionHelper.All).GetValue(this) + 1));
            }
        });
        #endregion
        #endregion
        
    }


}
