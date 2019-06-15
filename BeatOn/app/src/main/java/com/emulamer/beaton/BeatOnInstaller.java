package com.emulamer.beaton;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.util.Base64;

import com.google.common.collect.ImmutableList;

import org.jf.dexlib2.Opcode;
import org.jf.dexlib2.base.reference.BaseTypeReference;
import org.jf.dexlib2.iface.*;
import org.jf.dexlib2.iface.instruction.Instruction;
import org.jf.dexlib2.immutable.ImmutableMethod;
import org.jf.dexlib2.immutable.ImmutableMethodImplementation;
import org.jf.dexlib2.immutable.ImmutableMethodParameter;
import org.jf.dexlib2.immutable.ImmutableTryBlock;
import org.jf.dexlib2.immutable.debug.ImmutableDebugItem;
import org.jf.dexlib2.immutable.instruction.ImmutableInstruction;
import org.jf.dexlib2.immutable.instruction.ImmutableInstruction21c;
import org.jf.dexlib2.immutable.instruction.ImmutableInstruction35c;
import org.jf.dexlib2.immutable.reference.*;
import org.jf.dexlib2.iface.reference.*;
import org.jf.dexlib2.dexbacked.DexBackedClassDef;
import org.jf.dexlib2.dexbacked.DexBackedDexFile;
import org.jf.dexlib2.dexbacked.DexBackedMethod;
import org.jf.dexlib2.rewriter.DexRewriter;
import org.jf.dexlib2.rewriter.RewriterModule;

import beatonlib.beatonlib.BeatOnCore;
import dalvik.bytecode.Opcodes;
import questomassets.questomassets.Log;

import org.jf.dexlib2.rewriter.*;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.PrintStream;
import java.io.RandomAccessFile;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Random;
import java.util.Set;
import java.util.stream.IntStream;
import java.util.stream.Stream;
import org.jf.dexlib2.dexbacked.*;
import org.jf.dexlib2.writer.builder.BuilderAnnotationSet;
import org.jf.dexlib2.writer.builder.BuilderMethod;
import org.jf.dexlib2.writer.builder.BuilderMethodParameter;
import org.jf.dexlib2.writer.builder.BuilderMethodReference;
import org.jf.dexlib2.writer.io.MemoryDataStore;
import org.jf.dexlib2.writer.pool.DexPool;

import javax.annotation.Nonnull;
import javax.annotation.Nullable;

public class BeatOnInstaller {

    private PackageManager _packageManager;
    private Activity _context;
    public BeatOnInstaller(Activity context)
    {
        _context = context;
        _packageManager = context.getPackageManager();
    }

    public class InjectorClassDefRewrite extends ClassDefRewriter
    {
        public InjectorClassDefRewrite( Rewriters rewriters) {
            super(rewriters);

        }
        @Override
        public ClassDef rewrite( ClassDef classDef) {
            return new InjectorRewrittenClassDef(classDef);
        }

        protected class InjectorRewrittenClassDef extends RewrittenClassDef
        {
            public InjectorRewrittenClassDef(ClassDef classdef) {
                super(classdef);
            }

            @Override
            public Iterable<? extends Method> getDirectMethods()
            {
                if (!classDef.getType().equals("Lcom/unity3d/player/UnityPlayerActivity;"))
                    return super.getDirectMethods();

                List<Method> methods = new ArrayList<Method>();
                int accessFlags = 65544;
                BuilderAnnotationSet annotations = null;
                String definingClass = "Lcom/unity3d/player/UnityPlayerActivity;";
                String name = "<clinit>";
                Iterable<MethodParameter> parameters = new ArrayList<MethodParameter>();
                String returnType = "V";
                ImmutableMethodParameter parameter = new ImmutableMethodParameter("Ljava/lang/String;",null,null);
                List<ImmutableInstruction> instructions = Arrays.asList(
                        new ImmutableInstruction21c(Opcode.CONST_STRING, 0, new ImmutableStringReference("modloader")),
                        new ImmutableInstruction35c(Opcode.INVOKE_STATIC, 1, 0, 0, 0, 0, 0, new ImmutableMethodReference("Ljava/lang/System", "loadLibrary()", Arrays.asList(parameter), "V"))
                );
                ImmutableMethodImplementation methodImplementation = new ImmutableMethodImplementation(
                        1, instructions, null, null);

                Method staticInit = new ImmutableMethod(definingClass, name, parameters, returnType, accessFlags, annotations,
                methodImplementation);
                methods.add(staticInit);
                for (Method m : this.classDef.getDirectMethods())
                {
                    methods.add(m);
                }
                return methods;
            }
        }
    }

    public byte[] injectDex (byte[] classesDexBytes) throws Exception
    {
        ByteArrayInputStream bis = new ByteArrayInputStream(classesDexBytes);
        DexBackedDexFile dexFile = DexBackedDexFile.fromInputStream(null, bis);
        boolean foundStaticInit = false;
        for (DexBackedClassDef dexClass : dexFile.getClasses())
        {
            String type = dexClass.getType();
            if (type.equals("Lcom/unity3d/player/UnityPlayerActivity;"))
            {
                for (DexBackedMethod m : dexClass.getDirectMethods())
                {
                    String name =m.getName();
                    if (name.equals("<clinit>"))
                    {
                        //found a static class initializer, can't add another, means it's probably already injected
                        foundStaticInit = true;
                        break;
                    }
                }
                break;
            }
        }
        if (!foundStaticInit)
        {
            DexRewriter rewriter = new DexRewriter(new RewriterModule() {

                @Nonnull @Override public Rewriter<ClassDef> getClassDefRewriter(@Nonnull Rewriters rewriters) {
                    return new InjectorClassDefRewrite(rewriters);
                }
            });
            DexFile rewritten = rewriter.rewriteDexFile(dexFile);
            MemoryDataStore memStore = new MemoryDataStore();
            DexPool.writeTo(memStore, rewritten);
            return memStore.getData();
        }
        return null;
    }

    public byte[] getApkClassesDexBytes(String apkFileName)
    {
        BeatOnCore core = new BeatOnCore(apkFileName);
        try {
            String base64String = core.getBase64FileDataFromApk(apkFileName, "classes.dex");

            byte[] dexBytes = Base64.decode(base64String, 0);
            return dexBytes;
        }
        catch (Exception ex)
        {
            String logs = core.getLog();
            String lg = logs;
        }
        return null;
    }

    public void writeApkClassesDexBytes(String apkFileName, byte[] dexBytes) throws Exception
    {
        BeatOnCore core = new BeatOnCore(apkFileName);
        try {
           String base64String = Base64.encodeToString(dexBytes, 0);
           core.saveFileToApk(apkFileName, "classes.dex", base64String);
        } catch (Exception ex)
        {
            String logs = core.getLog();
            if (logs == null)
                logs = "(no logs)";
            throw new Exception("BeatOnCore Exception: " + logs);
        }
    }

    public String FindBeatSaberApk() throws Exception
    {
        byte[] dexBytes = getApkClassesDexBytes("/sdcard/Android/data/base.apk");
        byte[] injectedBytes = injectDex(dexBytes);

        if (injectedBytes != null)
        {
            writeApkClassesDexBytes("/sdcard/Android/data/base.apk", injectedBytes);
        }

        Intent mainIntent = new Intent(Intent.ACTION_MAIN, null);
        List pkgAppsList = _packageManager.queryIntentActivities(mainIntent, 0);
        for (Object object : pkgAppsList) {
            ResolveInfo info = (ResolveInfo) object;
            if (info.activityInfo.packageName == "com.beatgames.beatsaber")
            {
                //found beat saber
                return info.activityInfo.applicationInfo.publicSourceDir;
            }
        }
        return null;
    }

}
