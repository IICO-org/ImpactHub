
import { Link } from "react-router-dom";
import { MENU } from "./menu";
import type { MenuItem } from "./menu";
/**
 * D4 — Sidebar Builder
 *
 * Rules:
 * ------
 * 1) A module appears only if it exists in accessProfile.modules
 * 2) A menu item appears only if its requiredPermission exists in accessProfile.permissions
 *
 * This ensures the UI never exposes features the tenant does not own,
 * and never exposes actions the user is not allowed to perform.
 */
export function Sidebar(props: { modules: string[]; permissions: string[] }) {
    const modulesSet = new Set((props.modules ?? []).map((x) => x.toUpperCase()));
    const permsSet = new Set((props.permissions ?? []).map((x) => x.toUpperCase()));

    const isVisible = (item: MenuItem) => {
        if (!modulesSet.has(item.moduleCode.toUpperCase())) return false;
        if (!item.requiredPermission) return true;
        return permsSet.has(item.requiredPermission.toUpperCase());
    };

    const items = MENU.filter(isVisible);

    return (
        <div
            style={{
                width: 220,
                padding: 16,
                borderRight: "1px solid #ddd",
                height: "100vh",
                boxSizing: "border-box",
            }}
        >
            <div style={{ fontWeight: 700, marginBottom: 12 }}>ImpactHub</div>

            {items.length === 0 && (
                <div style={{ color: "#666" }}>No modules available for this user.</div>
            )}

            <ul style={{ listStyle: "none", padding: 0, margin: 0 }}>
                {items.map((x) => (
                    <li key={x.route} style={{ marginBottom: 10 }}>
                        <Link to={x.route} style={{ textDecoration: "none" }}>
                            {x.title}
                        </Link>
                    </li>
                ))}
            </ul>
        </div>
    );
}
