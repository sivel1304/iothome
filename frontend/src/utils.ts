// SQLite drops the "UTC" marker, so add it back or JS reads the time as local.
export const toDate = (ts: string) => new Date(ts.endsWith('Z') ? ts : ts + 'Z')

// A module counts as offline after missing about three reporting intervals.
export const isOffline = (lastSeen: string, intervalMs: number) =>
  Date.now() - toDate(lastSeen).getTime() > intervalMs * 3