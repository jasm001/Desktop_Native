# Admin Web Tests

The Block 7 control-plane tests live with the Node packages:

- `src/Contracts/Web/tests` for HTTP and outbox contracts;
- `src/AdminWeb/tests` for unit and PostgreSQL integration tests;
- `src/Worker/tests` for worker unit and PostgreSQL integration tests.

Block 11 is `in_progress`. Portal component and Playwright tests cover the
development-only administrative routes they verify; they are not production
role tests.

The current AdminWeb suite has 45 unit/component tests and 13 PostgreSQL
integration tests, including fail-closed portal authorization, bounded
administrative reads, omitted audit payloads, `portal.lab.read`,
`lab-real-sanitized` summaries, local connector health, curated lab catalog
validation, end-to-end lab traces, idempotency indicators, and absence of query
side effects. The direct PostgreSQL integration run needs a clean harness because
an append-only `audit_events` trigger can block setup cleanup; do not disable the
trigger only to force the test.
