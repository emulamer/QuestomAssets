package com.emulamer.beaton.dummy;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import android.view.View;

import com.emulamer.beaton.BeatOnInstaller;

/**
 * Helper class for providing sample content for user interfaces created by
 * Android template wizards.
 * <p>
 * TODO: Replace all uses of this class before publishing your app.
 */
public class DummyContent {

    /**
     * An array of sample (dummy) items.
     */
    public static final List<DummyItem> ITEMS = new ArrayList<DummyItem>();

    /**
     * A map of sample (dummy) items, by ID.
     */
    public static final Map<String, DummyItem> ITEM_MAP = new HashMap<String, DummyItem>();

    private static final int COUNT = 25;

    static {
        // Add some sample items.
        addItem(createDummyItem("Step 1", 1));
        addItem(createDummyItem("Step 2", 2));
    }

    private static void addItem(DummyItem item) {
        ITEMS.add(item);
        ITEM_MAP.put(item.id, item);
    }

    private static DummyItem createDummyItem(String text, int step) {
        return new DummyItem(text, text, makeDetails(text), step);
    }

    private static String makeDetails(String position) {
        StringBuilder builder = new StringBuilder();
        if (position.equals("Step 1")) {
            builder.append("Prepare and uninstall Beat Saber");
            builder.append("Assumes you have Beat Saber (unmodded?) already installed.\n");
            builder.append("Will grab the installed apk, copy it to the sdcard then trigger it to be uninstalled.\n");
            builder.append("Make sure to say yes to uninstall it");
        }
        else if (position.equals("Step 2")) {
            builder.append("Mod Beat Saber APK and Reinstall It");
            builder.append("Applies the preperatory mod to the Beat Saber APK and extracs the resources to the sdcard");
            builder.append("Takes the modded APK and and triggers it to be installed.");
            builder.append("Say yes to install it.");
        }

        return builder.toString();
    }

    /**
     * A dummy item representing a piece of content.
     */
    public static class DummyItem {
        public final String id;
        public final String content;
        public final String details;
        public int step;

        public DummyItem(String id, String content, String details, int step) {
            this.id = id;
            this.content = content;
            this.details = details;
            this.step = step;
        }

        @Override
        public String toString() {
            return content;
        }
    }


}
