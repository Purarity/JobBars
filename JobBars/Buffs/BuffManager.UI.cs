﻿using ImGuiNET;
using JobBars.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace JobBars.Buffs {
    public partial class BuffManager {
        private bool LOCKED = true;
        private JobIds SettingsJobSelected = JobIds.OTHER;

        public void Draw() {
            string _ID = "##JobBars_Buffs";

            ImGui.Checkbox("Locked" + _ID, ref LOCKED);

            ImGui.SameLine();
            if (ImGui.Checkbox("Buff Bar Enabled" + _ID, ref Configuration.Config.BuffBarEnabled)) {
                Configuration.Config.Save();
                if (Configuration.Config.BuffBarEnabled) UI.ShowBuffs();
                else UI.HideBuffs();
            }

            if (ImGui.InputFloat("Scale" + _ID, ref Configuration.Config.BuffScale)) {
                SetPositionScale();
                Configuration.Config.Save();
            }

            JobBars.Separator(); // =====================================

            if (ImGui.Checkbox("Hide Buffs When Out Of Combat", ref Configuration.Config.BuffHideOutOfCombat)) {
                if (!Configuration.Config.BuffHideOutOfCombat && Configuration.Config.BuffBarEnabled) { // since they might be hidden
                    UI.ShowBuffs();
                }
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Show Party Members' CDs and Buffs", ref Configuration.Config.BuffIncludeParty)) {
                Reset();
                Configuration.Config.Save();
            }

            JobBars.Separator(); // =====================================

            ImGui.SetNextItemWidth(25f);
            if (ImGui.InputInt("Buffs Per Line" + _ID, ref Configuration.Config.BuffHorizontal, 0)) {
                Configuration.Config.Save();
            }

            if (ImGui.Checkbox("Right-to-Left" + _ID, ref Configuration.Config.BuffRightToLeft)) {
                Configuration.Config.Save();
            }

            ImGui.SameLine();
            if (ImGui.Checkbox("Bottom-to-Top" + _ID, ref Configuration.Config.BuffBottomToTop)) {
                Configuration.Config.Save();
            }

            var size = ImGui.GetContentRegionAvail();
            ImGui.BeginChild(_ID + "/Child", size, true);
            ImGui.Columns(2);
            ImGui.SetColumnWidth(0, 150);

            ImGui.BeginChild(_ID + "Tree");
            foreach (var job in JobToBuffs.Keys) {
                if (job == JobIds.OTHER) continue;
                if (ImGui.Selectable(job + _ID + "/Job", SettingsJobSelected == job)) {
                    SettingsJobSelected = job;
                }
            }
            ImGui.EndChild();
            ImGui.NextColumn();

            if (SettingsJobSelected == JobIds.OTHER) {
                ImGui.Text("Select a job...");
            }
            else {
                ImGui.BeginChild(_ID + "Selected");
                foreach (var buff in JobToBuffs[SettingsJobSelected]) {
                    buff.Draw(_ID, SettingsJobSelected);
                }
                ImGui.EndChild();
            }
            ImGui.Columns(1);
            ImGui.EndChild();
        }

        public void DrawPositionBox() {
            if (!LOCKED) {
                if (JobBars.DrawPositionView("##BuffPosition", "Buff Bar", Configuration.Config.BuffPosition, out var pos)) {
                    SetBuffPosition(pos);
                }
            }
        }

        private void SetBuffPosition(Vector2 pos) {
            Configuration.Config.BuffPosition = pos;
            Configuration.Config.Save();
            UI?.SetBuffPosition(pos);
        }
    }
}
