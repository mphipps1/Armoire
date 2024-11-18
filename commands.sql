INSERT INTO Drawers (Id, Position, DrawerHierarchy, Name, ParentId)
VALUES ('CONTENTS_1', NULL, -1, 'Dock', NULL);

INSERT INTO Drawers (Id, Position, DrawerHierarchy, Name, ParentId)
VALUES ('CONTENTS_2', 0, 0, 'Drawer', 'CONTENTS_1');

INSERT INTO Drawers (Id, Position, DrawerHierarchy, Name, ParentId)
VALUES ('CONTENTS_4', 0, 1, 'Drawer', 'CONTENTS_2');

INSERT INTO Items (Id, Name, ExecutablePath, Position, DrawerHierarchy, ParentId)
VALUES ('CONTENTS_3', 'Notepad', 'C:\Windows\system32\notepad.exe', 1, 0, 'CONTENTS_1');
