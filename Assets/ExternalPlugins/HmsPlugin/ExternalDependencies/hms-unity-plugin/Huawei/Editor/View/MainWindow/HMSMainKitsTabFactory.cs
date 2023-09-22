using HmsPlugin;
using HmsPlugin.Button;
using HmsPlugin.HelpBox;
using HmsPlugin.Label;
using HmsPlugin.Toggle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modules.HmsPlugin.Editor;
using UnityEditor;
using UnityEngine;

internal class HMSMainKitsTabFactory
{
    private static string versionInfo = "";

    private static List<ToggleEditor> toggleEditors;
    public static DisabledDrawer _disabledDrawer;

    static HMSMainKitsTabFactory()
    {
        versionInfo = File.ReadAllText(HuaweiMobileServicesPluginHierarchy.Instance.VersionPath);
        toggleEditors = new List<ToggleEditor>();
    }

    public static TabView CreateTab(TabBar tabBar)
    {
        toggleEditors.Clear();
        var tab = new TabView("Kits");
        tabBar.AddTab(tab);

        var pluginToggleEditor = new PluginToggleEditor();
        var adsToggleEditor = new AdsToggleEditor(tabBar);
        var accountEditor = new AccountToggleEditor();
        var iapToggleEditor = new IAPToggleEditor(tabBar);

        tab.AddDrawer(new HorizontalSequenceDrawer(new Spacer(), pluginToggleEditor, new Spacer()));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(_disabledDrawer = new DisabledDrawer
            (
                new VerticalSequenceDrawer
                (
                    new HorizontalSequenceDrawer(new Spacer(), adsToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), iapToggleEditor, new Spacer()),
                    new HorizontalSequenceDrawer(new Spacer(), accountEditor, new Spacer())

                )
            ));
        tab.AddDrawer(new HorizontalLine());
        tab.AddDrawer(new Spacer());
        tab.AddDrawer(new Clickable(new Label("HMS Unity Plugin v" + versionInfo).SetBold(true), () => { Application.OpenURL("https://github.com/EvilMindDevs/hms-unity-plugin/"); }));
        tab.AddDrawer(new HelpboxAGConnectFile());

        toggleEditors.Add(adsToggleEditor);
        toggleEditors.Add(accountEditor);
        toggleEditors.Add(iapToggleEditor);
        _disabledDrawer.SetEnabled(!HMSPluginSettings.Instance.Settings.GetBool(PluginToggleEditor.PluginEnabled, true));

        return tab;
    }

    public static List<ToggleEditor> GetEnabledEditors()
    {
        return toggleEditors.FindAll(c => c.Enabled);
    }
}