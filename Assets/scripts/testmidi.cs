using System.Collections;
using System.Collections.Generic;
using System.Linq; // For Count()
using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System;

public class TestMidi : MonoBehaviour
{
    public string midiFilePath = "Assets/Midi/ADay.mid"; // adjust as needed

    private IEnumerable<Note> midiNotes;
    private TempoMap tempoMap;

    void Start()
    {
        try
        {
            MidiFile midiFile = MidiFile.Read(midiFilePath);
            tempoMap = midiFile.GetTempoMap();
            midiNotes = midiFile.GetNotes();
            Debug.Log("Loaded MIDI file with " + midiNotes.Count() + " note events.");

            foreach (var note in midiNotes)
            {
                // Convert note time (ticks) to MetricTimeSpan and then get total seconds.
                double noteStartSec = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap).TotalSeconds;
                Debug.Log($"Note {note.NoteNumber} starts at {noteStartSec} seconds");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error reading MIDI file: " + ex.Message);
        }
    }
}



