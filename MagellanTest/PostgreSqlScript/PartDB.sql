DROP DATABASE IF EXISTS Part;
CREATE DATABASE Part;

\c Part

DROP TABLE IF EXISTS item;
CREATE TABLE item (
	id SERIAL NOT NULL,
	item_name VARCHAR(50) NOT NULL,
	parent_item INTEGER,
	cost INTEGER NOT NULL,
	req_date DATE NOT NULL,
	PRIMARY KEY(id),
    CONSTRAINT fk_parent_item FOREIGN KEY(parent_item) REFERENCES item(id)
);

INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Item1', NULL, 500, '2024-02-20');
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Sub1', 1, 200, '2024-02-10');
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Sub2', 1, 300, '2024-01-05');
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Sub3', 2, 300, '2024-01-02');
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Sub4', 2, 400, '2024-01-02');
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Item2', NULL, 600, '2024-03-15');
INSERT INTO item (item_name, parent_item, cost, req_date) VALUES ('Sub1', 6, 200, '2024-02-25');

--TODO: format date to appear as MM-DD-YYYY using TO_CHAR

CREATE OR REPLACE FUNCTION Get_Total_Cost(name_of_item_to_find VARCHAR)
RETURNS INTEGER AS 
$$
DECLARE
    total_item_cost INTEGER;
BEGIN
    -- IF itemName's parent_item is NOT NULL Return NULL, ELSE calculate cost
    IF EXISTS(
        SELECT parent_item, item_name 
        FROM item 
        WHERE item_name = name_of_item_to_find AND parent_item IS NOT NULL
    ) THEN RETURN NULL;
    END IF;

    WITH RECURSIVE cte AS (
        SELECT id AS parent_item, item_name, cost
        FROM item
        WHERE item_name = name_of_item_to_find


        UNION ALL
        SELECT p.id, p.item_name, p.cost
        FROM cte
        JOIN item p USING (parent_item)
    )
    SELECT SUM(cost) INTO total_item_cost
    FROM cte;
    RETURN total_item_cost;
END;
$$ 
LANGUAGE plpgsql;