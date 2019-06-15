package com.emulamer.beaton;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.pm.ResolveInfo;
import android.net.Uri;
import android.util.Base64;
import com.google.common.io.Files;
import org.jf.dexlib2.Opcode;
import org.jf.dexlib2.iface.*;
import org.jf.dexlib2.immutable.ImmutableMethod;
import org.jf.dexlib2.immutable.ImmutableMethodImplementation;
import org.jf.dexlib2.immutable.ImmutableMethodParameter;
import org.jf.dexlib2.immutable.instruction.ImmutableInstruction;
import org.jf.dexlib2.immutable.instruction.ImmutableInstruction21c;
import org.jf.dexlib2.immutable.instruction.ImmutableInstruction35c;
import org.jf.dexlib2.immutable.reference.*;
import org.jf.dexlib2.dexbacked.DexBackedClassDef;
import org.jf.dexlib2.dexbacked.DexBackedDexFile;
import org.jf.dexlib2.dexbacked.DexBackedMethod;
import org.jf.dexlib2.rewriter.DexRewriter;
import org.jf.dexlib2.rewriter.RewriterModule;
import beatonlib.beatonlib.BeatOnCore;
import org.jf.dexlib2.rewriter.*;
import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;
import org.jf.dexlib2.writer.builder.BuilderAnnotationSet;
import org.jf.dexlib2.writer.io.MemoryDataStore;
import org.jf.dexlib2.writer.pool.DexPool;
import javax.annotation.Nonnull;


public class BeatOnInstaller {

    private PackageManager _packageManager;
    private Context _context;
    private File _tempApk;
    public BeatOnInstaller(Context context)
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

    public boolean modAndInstallBeatSaberApk() throws Exception
    {
        if (_tempApk == null)
        {
            //todo: inform/log they didn't do the first part
            return false;
        }
        //todo: wait for somebody to say ok to uninstall it
        if (findBeatSaberApk() != null)
        {
            //todo: inform/log that they didn't uninstall it!
           // return false;
        }

        String targetAssetsPath = "/sdcard/Android/data/com.beatgames.beatsaber/files/assets/";
        byte[] dexBytes = getApkClassesDexBytes(_tempApk.getAbsolutePath());
        byte[] injectedBytes = injectDex(dexBytes);

        if (injectedBytes != null) {
            writeApkClassesDexBytes(_tempApk.getAbsolutePath(), injectedBytes);
        } else {
            //todo: log the APK was probably already modified
        }

        //move asset files
        FileInputStream tempApkStream = new FileInputStream(_tempApk);
        ZipInputStream zipIs = new ZipInputStream(tempApkStream);
        ZipEntry ze = null;

        while ((ze = zipIs.getNextEntry()) != null) {

            FileOutputStream fout = new FileOutputStream(targetAssetsPath + ze.getName(),false);

            byte[] buffer = new byte[1024];
            int length = 0;

            while ((length = zipIs.read(buffer))>0) {
                fout.write(buffer, 0, length);
            }
            zipIs.closeEntry();
            fout.close();
        }
        zipIs.close();
        tempApkStream.close();
        BeatOnCore core = new BeatOnCore(_tempApk.getAbsolutePath());
        try {

            InputStream inp =_context.getResources().openRawResource(R.raw.libmodloader);
            byte[] data = new byte[inp.available()];
            inp.read(data);
            inp.close();
            core.saveFileToApk(_tempApk.getAbsolutePath(), "lib/armeabi-v7a/libmodloader.so", Base64.encodeToString(data,0));
        }
        catch (Exception ex)
        {
            //todo log error somehow
            throw new Exception(core.getLog());
        }

        InputStream inp2 =_context.getResources().openRawResource(R.raw.libassetredirect);
        byte[] data2 = new byte[inp2.available()];
        inp2.read(data2);
        inp2.close();
        FileOutputStream outp = new FileOutputStream("/sdcard/Android/data/com.beatgames.beatsaber/files/mods/libassetredirect.so",false);
        outp.write(data2);
        outp.close();

        Intent intent = new Intent(Intent.ACTION_VIEW);
        intent.setDataAndType(Uri.fromFile(_tempApk), "application/vnd.android.package-archive");
        _context.startActivity(intent);
        return true;
    }

    public boolean prepAndDeleteOriginalBeatSaberApk() throws Exception
    {

        String bsApkPath = findBeatSaberApk();
        if (bsApkPath == null) {
            //TODO: figure out how to log stuff in android
            return false;
        }
        _tempApk = new File(_context.getCacheDir(), "beatsabermod.apk");
        try {
            Files.copy(new File(bsApkPath), _tempApk);
            PackageManager pkgMgr = _context.getPackageManager();

            Intent intent = new Intent(Intent.ACTION_DELETE, Uri.fromParts("package",
                    pkgMgr.getPackageArchiveInfo(bsApkPath, 0).packageName,null));
            _context.startActivity(intent);

            return true;
        } catch (Exception ex)
        {
            _tempApk.delete();
            //TODO: how to log?
            throw ex;
        }
    }
    public String findBeatSaberApk() throws Exception
    {
        Intent mainIntent = new Intent(Intent.ACTION_MAIN, null);
        mainIntent.addCategory(Intent.CATEGORY_INFO);
        List pkgAppsList = _packageManager.queryIntentActivities(mainIntent, 0);
        for (Object object : pkgAppsList) {
            ResolveInfo info = (ResolveInfo) object;
            if (info.activityInfo.packageName.equals("com.beatgames.beatsaber"))
            {
                //found beat saber
                return info.activityInfo.applicationInfo.publicSourceDir;
            }
        }
        return null;
    }

}
