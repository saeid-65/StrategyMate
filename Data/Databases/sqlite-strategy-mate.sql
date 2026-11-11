-- ============================================================================
-- SQLite Schema — Approach B (numeric IDs only)
-- ============================================================================

PRAGMA foreign_keys = ON;

-- ============================================================================
-- Table: INDUSTRY_GROUPS
-- ============================================================================
CREATE TABLE IF NOT EXISTS INDUSTRY_GROUPS (
    ID          INTEGER PRIMARY KEY,
    GROUP_NAME  TEXT    NOT NULL,
    CREATED_AT  TIMESTAMP   DEFAULT CURRENT_TIMESTAMP NOT NULL,
    CONSTRAINT UX_INDUSTRY_GROUPS_NAME UNIQUE (GROUP_NAME)
);


-- ============================================================================
-- Table: INDUSTRY_SUB_GROUPS
-- ============================================================================
CREATE TABLE IF NOT EXISTS INDUSTRY_SUB_GROUPS (
    ID              INTEGER PRIMARY KEY,
    SUB_GROUP_NAME  TEXT    NOT NULL,
    GROUP_ID        INTEGER NOT NULL,
    CREATED_AT      TEXT    DEFAULT CURRENT_TIMESTAMP NOT NULL,

    CONSTRAINT UX_INDUSTRY_SUB_GROUPS_NAME UNIQUE (SUB_GROUP_NAME),

    CONSTRAINT FK_SUB_GROUP_GROUP FOREIGN KEY (GROUP_ID)
        REFERENCES INDUSTRY_GROUPS (ID) ON DELETE RESTRICT
);


-- ============================================================================
-- Table: SYMBOLS
-- ============================================================================
CREATE TABLE IF NOT EXISTS SYMBOLS (
    ID              INTEGER PRIMARY KEY,
    INSTRUMENT_CODE TEXT    NOT NULL,
    SYMBOL          TEXT    NOT NULL,
    COMPANY_NAME    TEXT    NOT NULL,
    ISIN            TEXT,
    GROUP_ID        INTEGER,
    SUB_GROUP_ID    INTEGER,
    MARKET          TEXT,
    BOARD           TEXT,
    IS_ACTIVE       INTEGER DEFAULT 1 NOT NULL,
    PAR_VALUE       NUMERIC DEFAULT 1000,
    TOTAL_SHARES    INTEGER,
    CREATED_AT      TEXT    DEFAULT CURRENT_TIMESTAMP NOT NULL,
    UPDATED_AT      TEXT    DEFAULT CURRENT_TIMESTAMP NOT NULL,

    CONSTRAINT UX_SYMBOLS_CODE    UNIQUE (INSTRUMENT_CODE),
    CONSTRAINT UX_SYMBOLS_SYMBOL  UNIQUE (SYMBOL),
    CONSTRAINT UX_SYMBOLS_ISIN    UNIQUE (ISIN),

    CONSTRAINT CHK_SYMBOLS_ACTIVE CHECK (IS_ACTIVE IN (0, 1)),
    CONSTRAINT CHK_SYMBOLS_MARKET CHECK (MARKET IS NULL OR MARKET IN ('BOURS', 'FARA', 'PAYEH')),
    CONSTRAINT CHK_SYMBOLS_BOARD  CHECK (BOARD IS NULL OR BOARD IN ('MAIN', 'SUB', 'BASE')),

    CONSTRAINT FK_SYMBOLS_GROUP     FOREIGN KEY (GROUP_ID)
        REFERENCES INDUSTRY_GROUPS (ID) ON DELETE RESTRICT,
    CONSTRAINT FK_SYMBOLS_SUB_GROUP FOREIGN KEY (SUB_GROUP_ID)
        REFERENCES INDUSTRY_SUB_GROUPS (ID) ON DELETE RESTRICT
);

-- ============================================================================
-- Indexes
-- ============================================================================
CREATE INDEX IF NOT EXISTS IX_SYMBOLS_GROUP         ON SYMBOLS (GROUP_ID);
CREATE INDEX IF NOT EXISTS IX_SYMBOLS_SUB_GROUP     ON SYMBOLS (SUB_GROUP_ID);
CREATE INDEX IF NOT EXISTS IX_SYMBOLS_ACTIVE        ON SYMBOLS (IS_ACTIVE);
CREATE INDEX IF NOT EXISTS IX_SYMBOLS_ACTIVE_GROUP  ON SYMBOLS (IS_ACTIVE, GROUP_ID);

-- ============================================================================
-- Trigger: Auto-update UPDATED_AT on row modification
-- ============================================================================
CREATE TRIGGER IF NOT EXISTS TRG_SYMBOLS_UPDATED_AT
AFTER UPDATE ON SYMBOLS
FOR EACH ROW
BEGIN
    UPDATE SYMBOLS
    SET UPDATED_AT = CURRENT_TIMESTAMP
    WHERE ID = OLD.ID;
END;