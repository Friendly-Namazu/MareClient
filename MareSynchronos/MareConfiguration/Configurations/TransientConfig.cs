﻿using MareSynchronos.API.Data.Enum;

namespace MareSynchronos.MareConfiguration.Configurations;

public class TransientConfig : IMareConfiguration
{
    public Dictionary<string, TransientPlayerConfig> TransientConfigs { get; set; } = [];
    public int Version { get; set; } = 1;

    public class TransientPlayerConfig
    {
        public List<string> GlobalPersistentCache { get; set; } = [];
        public Dictionary<uint, List<string>> JobSpecificCache { get; set; } = [];
        public Dictionary<uint, List<string>> JobSpecificPetCache { get; set; } = [];

        public TransientPlayerConfig()
        {

        }

        private bool ElevateIfNeeded(uint jobId, string gamePath)
        {
            // check if it's in the job cache of other jobs and elevate if needed
            foreach (var kvp in JobSpecificCache)
            {
                if (kvp.Key == jobId) continue;

                // elevate if the gamepath is included somewhere else
                if (kvp.Value.Contains(gamePath, StringComparer.Ordinal))
                {
                    JobSpecificCache[kvp.Key].Remove(gamePath);
                    GlobalPersistentCache.Add(gamePath);
                    return true;
                }
            }

            return false;
        }

        public int RemovePath(string gamePath, ObjectKind objectKind)
        {
            int removedEntries = 0;
            if (objectKind == ObjectKind.Player)
            {
                if (GlobalPersistentCache.Remove(gamePath)) removedEntries++;
                foreach (var kvp in JobSpecificCache)
                {
                    if (kvp.Value.Remove(gamePath)) removedEntries++;
                }
            }
            if (objectKind == ObjectKind.Pet)
            {
                foreach (var kvp in JobSpecificPetCache)
                {
                    if (kvp.Value.Remove(gamePath)) removedEntries++;
                }
            }
            return removedEntries;
        }

        public void AddOrElevate(uint jobId, string gamePath)
        {
            // check if it's in the global cache, if yes, do nothing
            if (GlobalPersistentCache.Contains(gamePath, StringComparer.Ordinal))
            {
                return;
            }

            if (ElevateIfNeeded(jobId, gamePath)) return;

            // check if the jobid is already in the cache to start
            if (!JobSpecificCache.TryGetValue(jobId, out var jobCache))
            {
                JobSpecificCache[jobId] = jobCache = new();
            }

            // check if the path is already in the job specific cache
            if (!jobCache.Contains(gamePath, StringComparer.Ordinal))
            {
                jobCache.Add(gamePath);
            }
        }
    }
}
