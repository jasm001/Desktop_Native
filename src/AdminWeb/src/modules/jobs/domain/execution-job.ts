export interface ExecutionJob {
  readonly id: string;
  readonly actionId: string;
  readonly targetId: string;
  readonly targetVersion: string;
  readonly status: "queued" | "completed" | "failed";
}
