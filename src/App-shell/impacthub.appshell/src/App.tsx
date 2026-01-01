import { useEffect, useState, type ReactNode } from "react";
import { BrowserRouter, Routes, Route, Navigate, Link } from "react-router-dom";
import { Sidebar } from "./Sidebar";
import { RequireAccess } from "./RequireAccess";
import { Forbidden } from "./Forbidden";

// ✅ Local image (place it under src/assets/images)
import childImage from "./assets/images/insight-child.jpg";

const TOKEN_STORAGE_KEY = "impacthub.dev.access_token";
const RIGHT_PANEL_WIDTH = 320;

// Microsoft/D365-ish accent
const MS_BLUE = "var(--ms-blue)";
const BORDER = "var(--border)";
const BG = "var(--bg)";

/**
 * AccessProfile contract (matches GET /api/me/access-profile)
 * This is the SINGLE source of truth for authorization data in the UI.
 */
type AccessProfile = {
    userId: number;
    tenantId: string;
    provider: string;
    issuer: string;
    subjectId: string;
    roles: string[];
    permissions: string[];
    modules: string[];
};

type Experience = "Admin" | "Executive" | "Partner";

function resolveExperience(profile: AccessProfile): Experience {
    const perms = new Set((profile.permissions ?? []).map((x) => x.toUpperCase()));
    const mods = new Set((profile.modules ?? []).map((x) => x.toUpperCase()));

    if (perms.has("ROLES.MANAGE")) return "Admin";
    if (mods.has("PROJECTS")) return "Executive";
    return "Partner";
}

export default function App() {
    const [token, setToken] = useState<string>(() => localStorage.getItem(TOKEN_STORAGE_KEY) ?? "");
    const [accessProfile, setAccessProfile] = useState<AccessProfile | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const [isRightPanelPinned, setIsRightPanelPinned] = useState<boolean>(true);
    const [isRightPanelOpen, setIsRightPanelOpen] = useState<boolean>(true);

    const loadAccessProfile = async (t: string) => {
        setIsLoading(true);
        setError(null);

        try {
            const response = await fetch("https://localhost:7266/api/me/access-profile", {
                method: "GET",
                headers: {
                    Authorization: `Bearer ${t}`,
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                if (response.status === 401) {
                    localStorage.removeItem(TOKEN_STORAGE_KEY);
                    setToken("");
                    setAccessProfile(null);
                    throw new Error("Token expired or invalid (401). Paste a new token.");
                }
                throw new Error(`HTTP ${response.status}`);
            }

            const data: AccessProfile = await response.json();
            setAccessProfile(data);
        } catch (err: unknown) {
            setAccessProfile(null);
            if (err instanceof Error) setError(err.message);
            else setError("Unknown error occurred.");
        } finally {
            setIsLoading(false);
        }
    };

    useEffect(() => {
        if (token.trim().length > 0) {
            loadAccessProfile(token);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    const saveTokenAndLoad = async () => {
        const t = token.trim();
        localStorage.setItem(TOKEN_STORAGE_KEY, t);
        await loadAccessProfile(t);
    };

    const clearToken = () => {
        localStorage.removeItem(TOKEN_STORAGE_KEY);
        setToken("");
        setAccessProfile(null);
        setError(null);
    };

    if (!accessProfile) {
        return (
            <div style={{ padding: 24, fontFamily: "Segoe UI, Arial" }}>
                <h1 style={{ marginBottom: 8 }}>ImpactHub App Shell</h1>
                <p style={{ marginTop: 0, color: "var(--muted)" }}>
                    Dev Login (temporary). Paste a valid access token (no “Bearer”).
                </p>

                <textarea
                    style={{
                        width: "100%",
                        height: 120,
                        padding: 12,
                        borderRadius: 10,
                        border: `1px solid ${BORDER}`,
                        fontFamily: "Consolas, monospace",
                        boxSizing: "border-box",
                    }}
                    value={token}
                    onChange={(e) => setToken(e.target.value)}
                />

                <div style={{ marginTop: 12, display: "flex", gap: 8 }}>
                    <PrimaryButton onClick={saveTokenAndLoad} disabled={isLoading || token.trim().length === 0}>
                        Save Token & Load Profile
                    </PrimaryButton>
                    <SecondaryButton onClick={clearToken} disabled={isLoading}>
                        Clear Token
                    </SecondaryButton>
                </div>

                {isLoading && <p>Loading access profile…</p>}
                {error && <p style={{ color: "crimson" }}>{error}</p>}
            </div>
        );
    }

    const experience: Experience = resolveExperience(accessProfile);
    const reservedRightSpace = isRightPanelOpen && isRightPanelPinned ? RIGHT_PANEL_WIDTH : 0;

    return (
        <BrowserRouter>
            <div style={{ display: "flex", height: "100vh", background: BG, fontFamily: "Segoe UI, Arial" }}>
                <Sidebar modules={accessProfile.modules} permissions={accessProfile.permissions} />

                <div
                    style={{
                        flex: 1,
                        padding: 16,
                        overflow: "auto",
                        marginRight: reservedRightSpace,
                        boxSizing: "border-box",
                    }}
                >
                    {/* Shell Header */}
                    <div
                        style={{
                            height: 48,
                            background: "white",
                            border: `1px solid ${BORDER}`,
                            borderRadius: 12,
                            padding: "0 12px",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "space-between",
                            marginBottom: 16,
                            boxSizing: "border-box",
                        }}
                    >
                        <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
                            <div style={{ fontWeight: 900, color: MS_BLUE }}>ImpactHub</div>

                            <div
                                style={{
                                    fontSize: 12,
                                    padding: "4px 10px",
                                    borderRadius: 999,
                                    border: `1px solid ${BORDER}`,
                                    background: "var(--surface-2)",
                                    color: "var(--text)",
                                    fontWeight: 700,
                                }}
                            >
                                Experience: {experience}
                            </div>
                        </div>

                        <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                            <SecondaryButton onClick={() => loadAccessProfile(token)} disabled={isLoading}>
                                Refresh Profile
                            </SecondaryButton>

                            <SecondaryButton onClick={() => setIsRightPanelOpen((x: boolean) => !x)}>
                                {isRightPanelOpen ? "Hide Panel" : "Show Panel"}
                            </SecondaryButton>

                            <SecondaryButton onClick={clearToken}>Logout (Dev)</SecondaryButton>
                        </div>
                    </div>

                    <Routes>
                        <Route path="/" element={<Navigate to="/home" replace />} />
                        <Route path="/home" element={<ExperienceHome experience={experience} accessProfile={accessProfile} />} />

                        <Route
                            path="/users"
                            element={
                                <RequireAccess accessProfile={accessProfile} moduleCode="USERS" requiredPermission="USERS.VIEW">
                                    <UsersPage />
                                </RequireAccess>
                            }
                        />

                        <Route
                            path="/roles"
                            element={
                                <RequireAccess accessProfile={accessProfile} moduleCode="ROLES" requiredPermission="ROLES.MANAGE">
                                    <RolesPage />
                                </RequireAccess>
                            }
                        />

                        <Route
                            path="/projects"
                            element={
                                <RequireAccess accessProfile={accessProfile} moduleCode="PROJECTS">
                                    <ProjectsPage />
                                </RequireAccess>
                            }
                        />

                        <Route path="/forbidden" element={<Forbidden />} />
                        <Route path="*" element={<Placeholder title="Not Found" />} />
                    </Routes>
                </div>

                {/* RIGHT PANEL */}
                {isRightPanelOpen && (
                    <div
                        style={{
                            width: RIGHT_PANEL_WIDTH,
                            position: "fixed",
                            right: 0,
                            top: 0,

                            marginRight: isRightPanelPinned ? 0 : 12,
                            marginTop: isRightPanelPinned ? 0 : 12,
                            marginBottom: isRightPanelPinned ? 0 : 12,

                            height: isRightPanelPinned ? "100vh" : "calc(100vh - 24px)",
                            background: "white",

                            border: isRightPanelPinned ? "none" : `1px solid ${BORDER}`,
                            borderLeft: `1px solid ${BORDER}`,
                            borderRadius: isRightPanelPinned ? 0 : 12,

                            padding: 12,
                            boxSizing: "border-box",
                            boxShadow: isRightPanelPinned ? "none" : "var(--shadow)",
                        }}
                    >
                        <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                            <div style={{ fontWeight: 900, color: MS_BLUE }}>Insights</div>

                            <div style={{ display: "flex", gap: 8 }}>
                                <SecondaryButton onClick={() => setIsRightPanelPinned((x: boolean) => !x)}>
                                    {isRightPanelPinned ? "Unpin" : "Pin"}
                                </SecondaryButton>
                                <SecondaryButton onClick={() => setIsRightPanelOpen(false)}>X</SecondaryButton>
                            </div>
                        </div>

                        {/* Card diversity as requested */}
                        <div style={{ marginTop: 12, display: "grid", gap: 12 }}>
                            {/* 1) Text-only card */}
                            <VivaTextCard title="Focus time every day">
                                Reserve distraction-free deep work blocks.
                            </VivaTextCard>

                            {/* 2) Image + text card (LOCAL IMAGE) */}
                            <VivaImageCard title="Wellbeing" imageUrl={childImage}>
                                Every child deserves safety, dignity, and a future filled with hope.
                            </VivaImageCard>

                            {/* 3) Text + small button AFTER text */}
                            <VivaActionCard title="Quick setup" buttonText="Open" onClick={() => alert("Action (placeholder)")}>
                                Configure your notification and focus preferences.
                            </VivaActionCard>
                        </div>
                    </div>
                )}
            </div>
        </BrowserRouter>
    );
}

/* =========================
   D6: EXPERIENCE HOME
   ========================= */
function ExperienceHome(props: { experience: Experience; accessProfile: AccessProfile }) {
    const { experience, accessProfile } = props;

    const summary = [
        { label: "Modules", value: accessProfile.modules.length.toString() },
        { label: "Permissions", value: accessProfile.permissions.length.toString() },
        { label: "Roles", value: accessProfile.roles.length.toString() },
    ];

    return (
        <PageShell
            title={`${experience} Home`}
            subtitle="Your landing experience is derived from enabled modules + granted permissions."
            actions={<SecondaryButton onClick={() => alert("Placeholder")}>Switch</SecondaryButton>}
        >
            <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12 }}>
                {summary.map((x) => (
                    <div key={x.label} className="surface" style={{ padding: 12 }}>
                        <div style={{ color: "var(--muted)", fontSize: 12, fontWeight: 800 }}>{x.label}</div>
                        <div style={{ fontSize: 22, fontWeight: 900, color: "var(--ms-blue)" }}>{x.value}</div>
                    </div>
                ))}
            </div>

            <div style={{ marginTop: 14 }}>
                <div style={{ fontWeight: 900, marginBottom: 8 }}>Quick Links</div>

                <div style={{ display: "flex", gap: 10, flexWrap: "wrap" }}>
                    <QuickLink to="/users" title="Users" />
                    <QuickLink to="/roles" title="Roles & Permissions" />
                    <QuickLink to="/projects" title="Projects" />
                </div>
            </div>
        </PageShell>
    );
}

function QuickLink(props: { to: string; title: string }) {
    return (
        <Link
            to={props.to}
            style={{
                textDecoration: "none",
                color: "var(--text)",
                border: `1px solid var(--border)`,
                background: "white",
                borderRadius: 12,
                padding: "10px 12px",
                fontWeight: 800,
            }}
        >
            {props.title}
        </Link>
    );
}

/* =========================
   D7 PAGES (still skeletons)
   ========================= */

function UsersPage() {
    return (
        <PageShell title="Users" subtitle="Manage users and access." actions={<PrimaryButton onClick={() => alert("Add user")}>Add User</PrimaryButton>}>
            <Toolbar>
                <SearchInput placeholder="Search users..." />
                <FilterSelect options={[{ value: "all", label: "All" }]} />
                <SecondaryButton onClick={() => alert("Refresh")}>Refresh</SecondaryButton>
            </Toolbar>

            <FakeTable
                columns={["Name", "Email", "Status", "Actions"]}
                rows={Array.from({ length: 6 }).map((_, i) => [
                    `User ${i + 1}`,
                    `user${i + 1}@example.com`,
                    "Active",
                    <div key={i} style={{ display: "flex", gap: 8 }}>
                        <SecondaryButton>Edit</SecondaryButton>
                        <SecondaryButton>Details</SecondaryButton>
                    </div>,
                ])}
            />
        </PageShell>
    );
}

function RolesPage() {
    return (
        <PageShell title="Roles & Permissions" subtitle="Manage roles and permission mappings." actions={<PrimaryButton onClick={() => alert("Create role")}>Create Role</PrimaryButton>}>
            <Toolbar>
                <SearchInput placeholder="Search roles..." />
                <FilterSelect options={[{ value: "all", label: "All" }]} />
                <SecondaryButton onClick={() => alert("Refresh")}>Refresh</SecondaryButton>
            </Toolbar>

            <FakeTable
                columns={["Role Code", "Name", "Permissions", "Actions"]}
                rows={[
                    ["HQ_ADMIN", "HQ Administrator", "4", <SecondaryButton key="a">Permissions</SecondaryButton>],
                    ["HQ_REVIEWER", "HQ Reviewer", "3", <SecondaryButton key="b">Permissions</SecondaryButton>],
                ]}
            />
        </PageShell>
    );
}

function ProjectsPage() {
    return (
        <PageShell title="Projects" subtitle="Browse projects and track progress." actions={<PrimaryButton onClick={() => alert("New project")}>New Project</PrimaryButton>}>
            <Toolbar>
                <SearchInput placeholder="Search projects..." />
                <FilterSelect options={[{ value: "all", label: "All" }]} />
                <SecondaryButton onClick={() => alert("Refresh")}>Refresh</SecondaryButton>
            </Toolbar>

            <FakeTable
                columns={["Project", "Country", "Status", "Actions"]}
                rows={[
                    ["Education Support 2026", "Egypt", "Active", <SecondaryButton key="p1">Open</SecondaryButton>],
                    ["Water Wells Program", "Kenya", "Active", <SecondaryButton key="p2">Open</SecondaryButton>],
                ]}
            />
        </PageShell>
    );
}

/* =========================
   UI building blocks
   ========================= */

function PageShell(props: { title: string; subtitle: string; actions?: ReactNode; children: ReactNode }) {
    return (
        <div className="surface" style={{ width: "100%", minHeight: "calc(100vh - 120px)", padding: 16, boxSizing: "border-box" }}>
            <div style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-start" }}>
                <div>
                    <h2 style={{ margin: 0 }}>{props.title}</h2>
                    <div style={{ color: "var(--muted)", fontSize: 13, marginTop: 4 }}>{props.subtitle}</div>
                </div>

                <div style={{ display: "flex", gap: 8 }}>{props.actions}</div>
            </div>

            <div style={{ marginTop: 14 }}>{props.children}</div>
        </div>
    );
}

function Toolbar(props: { children: ReactNode }) {
    return (
        <div className="surface" style={{ padding: 10, borderRadius: 12, background: "var(--surface-2)", display: "flex", gap: 10, alignItems: "center" }}>
            {props.children}
        </div>
    );
}

function SearchInput(props: { placeholder: string }) {
    return <input placeholder={props.placeholder} style={{ flex: 1 }} />;
}

function FilterSelect(props: { options: Array<{ value: string; label: string }> }) {
    return (
        <select defaultValue={props.options[0]?.value ?? "all"}>
            {props.options.map((o) => (
                <option key={o.value} value={o.value}>
                    {o.label}
                </option>
            ))}
        </select>
    );
}

function FakeTable(props: { columns: string[]; rows: Array<Array<string | ReactNode>> }) {
    return (
        <div className="surface" style={{ marginTop: 14, borderRadius: 12, overflow: "hidden" }}>
            <div
                style={{
                    display: "grid",
                    gridTemplateColumns: `repeat(${props.columns.length}, 1fr)`,
                    background: "var(--bg)",
                    borderBottom: `1px solid var(--border)`,
                    padding: "10px 12px",
                    fontWeight: 900,
                    fontSize: 13,
                }}
            >
                {props.columns.map((c) => (
                    <div key={c}>{c}</div>
                ))}
            </div>

            {props.rows.map((r, i) => (
                <div
                    key={i}
                    style={{
                        display: "grid",
                        gridTemplateColumns: `repeat(${props.columns.length}, 1fr)`,
                        padding: "10px 12px",
                        borderBottom: i === props.rows.length - 1 ? "none" : "1px solid #eef2f6",
                        fontSize: 13,
                        alignItems: "center",
                    }}
                >
                    {r.map((cell, idx) => (
                        <div key={idx}>{cell}</div>
                    ))}
                </div>
            ))}
        </div>
    );
}

function PrimaryButton(props: { children: ReactNode; onClick?: () => void; disabled?: boolean }) {
    return (
        <button className="btn-primary" onClick={props.onClick} disabled={props.disabled}>
            {props.children}
        </button>
    );
}

function SecondaryButton(props: { children: ReactNode; onClick?: () => void; disabled?: boolean }) {
    return (
        <button onClick={props.onClick} disabled={props.disabled}>
            {props.children}
        </button>
    );
}

/* =========================
   Viva-like diverse cards
   ========================= */

function VivaTextCard(props: { title: string; children: ReactNode }) {
    return (
        <div className="surface" style={{ padding: 12 }}>
            <div style={{ fontWeight: 900, color: "var(--ms-blue)", marginBottom: 6 }}>{props.title}</div>
            <div style={{ color: "var(--muted)", fontSize: 13 }}>{props.children}</div>
        </div>
    );
}

/** ✅ Image + text below image */
function VivaImageCard(props: { title: string; imageUrl: string; children: ReactNode }) {
    return (
        <div className="surface" style={{ padding: 12 }}>
            <div style={{ fontWeight: 900, color: "var(--ms-blue)", marginBottom: 8 }}>{props.title}</div>

            <img
                src={props.imageUrl}
                alt={props.title}
                style={{
                    width: "100%",
                    height: 160,
                    objectFit: "cover",
                    borderRadius: 10,
                    border: "1px solid var(--border)",
                }}
            />

            <div style={{ marginTop: 10, color: "var(--muted)", fontSize: 13, lineHeight: 1.5 }}>
                {props.children}
            </div>
        </div>
    );
}

/** ✅ Button AFTER text (not top-right) */
function VivaActionCard(props: { title: string; buttonText: string; onClick: () => void; children: ReactNode }) {
    return (
        <div className="surface" style={{ padding: 12 }}>
            <div style={{ fontWeight: 900, color: "var(--ms-blue)" }}>{props.title}</div>

            <div style={{ marginTop: 8, color: "var(--muted)", fontSize: 13, lineHeight: 1.5 }}>
                {props.children}
            </div>

            <div style={{ marginTop: 12 }}>
                <button
                    onClick={props.onClick}
                    style={{
                        padding: "6px 14px",
                        borderRadius: 10,
                        border: "1px solid var(--border)",
                        background: "white",
                        fontWeight: 700,
                    }}
                >
                    {props.buttonText}
                </button>
            </div>
        </div>
    );
}

function Placeholder(props: { title: string }) {
    return (
        <div className="surface" style={{ padding: 16, minHeight: "calc(100vh - 120px)", boxSizing: "border-box" }}>
            <h2 style={{ marginTop: 0 }}>{props.title}</h2>
            <p style={{ color: "var(--muted)" }}>Placeholder screen.</p>
        </div>
    );
}
