/**
 * D5 — Forbidden Page
 *
 * This is where we send the user when:
 * - the tenant does not own a module (feature not enabled), OR
 * - the user lacks the permission for a route/action
 */
export function Forbidden() {
    return (
        <div
            style={{
                background: "white",
                border: "1px solid #e5e5e5",
                borderRadius: 12,
                padding: 16,
            }}
        >
            <h2 style={{ marginTop: 0 }}>403 – Forbidden</h2>
            <p style={{ color: "#555" }}>
                You do not have access to this page (module disabled or permission missing).
            </p>
        </div>
    );
}
