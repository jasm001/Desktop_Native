# Admin Web Tests

The Block 7 control-plane tests live with the Node packages:

- `src/Contracts/Web/tests` for HTTP and outbox contracts;
- `src/AdminWeb/tests` for unit and PostgreSQL integration tests;
- `src/Worker/tests` for worker unit and PostgreSQL integration tests.

Block 11 is `in_progress`. Portal component and Playwright role tests do not
exist yet and must be introduced with the portal capabilities they verify.
The current AdminWeb suite has 22 unit tests and 12 PostgreSQL integration
tests, including fail-closed portal authorization, bounded administrative
reads, omitted audit payloads, and absence of query side effects.
