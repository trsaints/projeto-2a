CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "Events" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Events" PRIMARY KEY AUTOINCREMENT,
    "AgendaName" TEXT NULL,
    "Name" TEXT NOT NULL,
    "Repeats" INTEGER NOT NULL,
    "Reminders" TEXT NULL,
    "InitialDue" TEXT NOT NULL,
    "Due" TEXT NOT NULL,
    "Description" TEXT NULL
);

CREATE TABLE "Todos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Todos" PRIMARY KEY AUTOINCREMENT,
    "ListName" TEXT NULL,
    "FinishedShifts" INTEGER NOT NULL,
    "TotalShifts" INTEGER NOT NULL,
    "EventId" INTEGER NULL,
    "Status" INTEGER NOT NULL,
    "Name" TEXT NOT NULL,
    "Repeats" INTEGER NOT NULL,
    "Reminders" TEXT NULL,
    "InitialDue" TEXT NOT NULL,
    "Due" TEXT NOT NULL,
    "Description" TEXT NULL,
    CONSTRAINT "FK_Todos_Events_EventId" FOREIGN KEY ("EventId") REFERENCES "Events" ("Id")
);

CREATE TABLE "Shifts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Shifts" PRIMARY KEY AUTOINCREMENT,
    "Duration" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "TodoId" INTEGER NULL,
    "Name" TEXT NOT NULL,
    CONSTRAINT "FK_Shifts_Todos_TodoId" FOREIGN KEY ("TodoId") REFERENCES "Todos" ("Id")
);

CREATE INDEX "IX_Shifts_TodoId" ON "Shifts" ("TodoId");

CREATE INDEX "IX_Todos_EventId" ON "Todos" ("EventId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250615140821_Schema', '9.0.3');

COMMIT;


