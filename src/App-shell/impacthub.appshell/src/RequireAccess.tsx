import { Navigate } from "react-router-dom";

/**
 * D5 — Route Guard (Feature + Permission)
 *
 * Why this exists:
 * ---------------
 * Sidebar hiding is NOT enough.
 * Users can deep-link directly to a URL (e.g. /roles).
 *
 * This guard enforces the same rules as the sidebar:
 * - Module must be enabled (tenant features)
 * - Permission must be granted (user authorization)
 *
 * Behavior:
 * ---------
 * - If module missing -> redirect to /forbidden
 * - If permission missing -> redirect to /forbidden
 */
export function RequireAccess(props: {
    accessProfile: any;
    moduleCode: string;
    requiredPermission?: string;
    children: React.ReactNode;
}) {
    const modules: string[] = props.accessProfile?.modules ?? [];
    const permissions: string[] = props.accessProfile?.permissions ?? [];

    const modulesSet = new Set(modules.map((x) => x.toUpperCase()));
    const permsSet = new Set(permissions.map((x) => x.toUpperCase()));

    const hasModule = modulesSet.has(props.moduleCode.toUpperCase());
    if (!hasModule) {
        return <Navigate to="/forbidden" replace />;
    }

    if (props.requiredPermission) {
        const hasPerm = permsSet.has(props.requiredPermission.toUpperCase());
        if (!hasPerm) {
            return <Navigate to="/forbidden" replace />;
        }
    }

    return <>{props.children}</>;
}
