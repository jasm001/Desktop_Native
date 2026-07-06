# Admin Web Tests

The Block 7 control-plane tests live with the Node packages:

- `src/Contracts/Web/tests` for HTTP and outbox contracts;
- `src/AdminWeb/tests` for unit and PostgreSQL integration tests;
- `src/Worker/tests` for worker unit and PostgreSQL integration tests.

Block 11 is `in_progress`. Portal component and Playwright tests cover the
development-only administrative routes they verify; they are not production
role tests.

The current AdminWeb suite has 40 unit/component tests and 13 PostgreSQL
integration tests, including fail-closed portal authorization, bounded
administrative reads, omitted audit payloads, `portal.lab.read`,
`lab-real-sanitized` summaries, local connector health, and absence of query
side effects.
