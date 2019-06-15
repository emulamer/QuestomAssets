package com.emulamer.beaton;

import android.util.Log;
import android.view.View;

public class ClickHandler implements View.OnClickListener
{
    private static BeatOnInstaller _installer;
    int step;
    public ClickHandler(int step) {
        this.step= step;
    }

    @Override
    public void onClick(View v)
    {
        if (step == 1)
        {
            if (_installer != null) {
                //it's already tried once
                //TODO: error or something
                return;
            }
                try {
                    _installer = new BeatOnInstaller(v.getContext());
                    _installer.prepAndDeleteOriginalBeatSaberApk();
                }
                catch (Exception ex)
                {
                    //TODO: error or something
                }
        }
        else if (step == 2)
        {
            if (_installer == null)
            {
                //TODO: show an error about having to do step 1 first
                return;
            }
            try {
                if (!_installer.modAndInstallBeatSaberApk()) {
                    //TODO: show an error it failed
                }
            }
            catch (Exception ex)
            {
                //TODO: show an error
                Log.w("error", ex.getMessage());
            }
        }
    }

};