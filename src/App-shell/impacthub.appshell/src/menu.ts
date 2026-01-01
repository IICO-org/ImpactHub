/**
 * D4 — App Shell Menu Contract (Single Source of Truth)
 *
 * This file defines the entire UI navigation model:
 * - Which modules exist (USERS, ROLES, PROJECTS, ...)
 * - Which routes belong to each module
 * - Which permission is required to see each menu item
 *
 * IMPORTANT:
 * ----------
 * The sidebar must NOT be hardcoded elsewhere.
 * It must be generated from this config + accessProfile (modules + permissions).
 */

export type MenuItem = {
    moduleCode: string;
    title: string;
    route: string;

    /**
     * Permission required to show the menu item.
     * Must match iam.Permissions.Code exactly.
     *
     * If omitted, the item is gated only by moduleCode.
     * (We try to avoid this in ERP apps; explicit is better.)
     */
    requiredPermission?: string;
};

export const MENU: MenuItem[] = [
    {
        moduleCode: "USERS",
        title: "Users",
        route: "/users",
        requiredPermission: "USERS.VIEW",
    },
    {
        moduleCode: "ROLES",
        title: "Roles & Permissions",
        route: "/roles",
        requiredPermission: "ROLES.MANAGE",
    },
    {
        moduleCode: "PROJECTS",
        title: "Projects",
        route: "/projects",
        // You don't currently have PROJECTS.VIEW in iam.Permissions,
        // so we gate Projects only by the enabled module for now.
    },
];
